#!/usr/bin/env python3
"""
Config Validator for TopDeck Unity Game
Validates ScriptableObject configurations and game parameters.
Integrates with Unity MCP to check asset validity and ranges.
"""

import json
import sys
import re
from typing import Dict, List, Any, Tuple
from dataclasses import dataclass
from enum import Enum


class ValidationLevel(Enum):
    """Severity levels for validation issues"""
    ERROR = "ERROR"
    WARNING = "WARNING"
    INFO = "INFO"


@dataclass
class ValidationResult:
    """Result of a validation check"""
    level: ValidationLevel
    field: str
    message: str
    suggestion: str = ""


class ConfigValidator:
    """Validates Unity configuration files and ScriptableObject data"""
    
    def __init__(self):
        self.results: List[ValidationResult] = []
        
        # Define validation rules for different config types
        self.rules = {
            "wave_difficulty": {
                "min_enemy_count": (1, 100),
                "max_enemy_count": (1, 500),
                "spawn_interval": (0.1, 10.0),
                "difficulty_multiplier": (0.1, 10.0),
                "elite_chance": (0.0, 1.0),
                "mini_boss_chance": (0.0, 0.5)
            },
            "upgrade_costs": {
                "base_cost": (10, 1000),
                "cost_multiplier": (1.1, 3.0),
                "max_level": (1, 10)
            },
            "defender_stats": {
                "health": (10, 10000),
                "damage": (1, 1000),
                "fire_rate": (0.1, 10.0),
                "range": (1.0, 50.0)
            },
            "enemy_variants": {
                "health_multiplier": (0.5, 5.0),
                "speed_multiplier": (0.5, 3.0),
                "damage_multiplier": (0.5, 5.0),
                "reward_multiplier": (0.5, 10.0)
            },
            "shader_params": {
                "vertex_displacement": (0.0, 2.0),
                "color_intensity": (0.0, 1.0),
                "emission_strength": (0.0, 10.0),
                "pulse_frequency": (0.0, 10.0)
            }
        }
    
    def validate_range(self, value: float, min_val: float, max_val: float, field: str) -> bool:
        """Check if a value is within acceptable range"""
        if value < min_val:
            self.results.append(ValidationResult(
                level=ValidationLevel.ERROR,
                field=field,
                message=f"Value {value} is below minimum {min_val}",
                suggestion=f"Set {field} to at least {min_val}"
            ))
            return False
        elif value > max_val:
            self.results.append(ValidationResult(
                level=ValidationLevel.WARNING,
                field=field,
                message=f"Value {value} exceeds recommended maximum {max_val}",
                suggestion=f"Consider keeping {field} below {max_val} for balance"
            ))
            return False
        return True
    
    def validate_progression(self, values: List[float], field: str) -> bool:
        """Validate that a list of values forms a proper progression"""
        if not values:
            return True
            
        for i in range(1, len(values)):
            if values[i] < values[i-1]:
                self.results.append(ValidationResult(
                    level=ValidationLevel.WARNING,
                    field=field,
                    message=f"Non-increasing progression at index {i}: {values[i]} < {values[i-1]}",
                    suggestion="Ensure values increase or stay constant for proper difficulty scaling"
                ))
                return False
        return True
    
    def validate_config(self, config_type: str, data: Dict[str, Any]) -> List[ValidationResult]:
        """Validate a configuration based on its type"""
        self.results = []
        
        if config_type not in self.rules:
            self.results.append(ValidationResult(
                level=ValidationLevel.INFO,
                field="config_type",
                message=f"No specific rules defined for {config_type}",
                suggestion="Using general validation only"
            ))
            return self.results
        
        rules = self.rules[config_type]
        
        for field, value in data.items():
            if field in rules:
                min_val, max_val = rules[field]
                
                if isinstance(value, list):
                    # Validate each element in the list
                    for i, v in enumerate(value):
                        self.validate_range(v, min_val, max_val, f"{field}[{i}]")
                    # Also validate progression
                    self.validate_progression(value, field)
                else:
                    self.validate_range(value, min_val, max_val, field)
        
        # Check for missing required fields
        for field in rules:
            if field not in data:
                self.results.append(ValidationResult(
                    level=ValidationLevel.WARNING,
                    field=field,
                    message=f"Missing expected field: {field}",
                    suggestion=f"Add {field} to configuration"
                ))
        
        return self.results
    
    def validate_balance(self, upgrade_config: Dict, wave_config: Dict) -> List[ValidationResult]:
        """Cross-validate upgrade and wave configs for game balance"""
        self.results = []
        
        # Check if max upgrades can handle max wave difficulty
        max_health_mult = upgrade_config.get("max_health_multiplier", 1.0)
        max_damage_mult = upgrade_config.get("max_damage_multiplier", 1.0)
        max_enemy_health = wave_config.get("max_enemy_health_multiplier", 1.0)
        max_enemy_count = wave_config.get("max_enemy_count", 100)
        
        power_ratio = (max_health_mult * max_damage_mult) / (max_enemy_health * (max_enemy_count / 10))
        
        if power_ratio < 0.5:
            self.results.append(ValidationResult(
                level=ValidationLevel.ERROR,
                field="balance",
                message="Upgrades insufficient for late-game waves",
                suggestion="Increase upgrade multipliers or reduce late-wave difficulty"
            ))
        elif power_ratio > 3.0:
            self.results.append(ValidationResult(
                level=ValidationLevel.WARNING,
                field="balance",
                message="Upgrades may make late-game too easy",
                suggestion="Consider reducing upgrade power or increasing wave difficulty"
            ))
        
        # Check economy balance
        total_cost = upgrade_config.get("total_upgrade_cost", 10000)
        total_rewards = wave_config.get("estimated_total_rewards", 5000)
        
        if total_cost > total_rewards * 1.5:
            self.results.append(ValidationResult(
                level=ValidationLevel.WARNING,
                field="economy",
                message="Upgrade costs exceed expected earnings",
                suggestion="Reduce upgrade costs or increase enemy rewards"
            ))
        
        return self.results
    
    def generate_report(self) -> str:
        """Generate a formatted validation report"""
        if not self.results:
            return "✅ All validations passed!"
        
        report = ["=" * 60, "VALIDATION REPORT", "=" * 60, ""]
        
        # Group by severity
        errors = [r for r in self.results if r.level == ValidationLevel.ERROR]
        warnings = [r for r in self.results if r.level == ValidationLevel.WARNING]
        info = [r for r in self.results if r.level == ValidationLevel.INFO]
        
        if errors:
            report.append(f"❌ ERRORS ({len(errors)})")
            report.append("-" * 40)
            for r in errors:
                report.append(f"  [{r.field}] {r.message}")
                if r.suggestion:
                    report.append(f"    → {r.suggestion}")
            report.append("")
        
        if warnings:
            report.append(f"⚠️  WARNINGS ({len(warnings)})")
            report.append("-" * 40)
            for r in warnings:
                report.append(f"  [{r.field}] {r.message}")
                if r.suggestion:
                    report.append(f"    → {r.suggestion}")
            report.append("")
        
        if info:
            report.append(f"ℹ️  INFO ({len(info)})")
            report.append("-" * 40)
            for r in info:
                report.append(f"  [{r.field}] {r.message}")
            report.append("")
        
        # Summary
        report.append("=" * 60)
        report.append("SUMMARY")
        report.append(f"  Errors:   {len(errors)}")
        report.append(f"  Warnings: {len(warnings)}")
        report.append(f"  Info:     {len(info)}")
        
        status = "PASSED ✅" if not errors else "FAILED ❌"
        report.append(f"  Status:   {status}")
        report.append("=" * 60)
        
        return "\n".join(report)


def main(config_path: str = None):
    """Main entry point for config validation"""
    validator = ConfigValidator()
    
    # Example validation - in real use, this would read from Unity assets
    sample_wave_config = {
        "min_enemy_count": 5,
        "max_enemy_count": 150,
        "spawn_interval": 0.5,
        "difficulty_multiplier": 2.5,
        "elite_chance": 0.2,
        "mini_boss_chance": 0.1,
        "max_enemy_health_multiplier": 3.0,
        "estimated_total_rewards": 8000
    }
    
    sample_upgrade_config = {
        "base_cost": 100,
        "cost_multiplier": 1.5,
        "max_level": 5,
        "max_health_multiplier": 2.5,
        "max_damage_multiplier": 2.0,
        "total_upgrade_cost": 12000
    }
    
    sample_defender_config = {
        "health": 100,
        "damage": 25,
        "fire_rate": 1.5,
        "range": 10.0
    }
    
    print("Validating Wave Configuration...")
    validator.validate_config("wave_difficulty", sample_wave_config)
    
    print("Validating Upgrade Configuration...")
    validator.validate_config("upgrade_costs", sample_upgrade_config)
    
    print("Validating Defender Configuration...")
    validator.validate_config("defender_stats", sample_defender_config)
    
    print("Validating Game Balance...")
    validator.validate_balance(sample_upgrade_config, sample_wave_config)
    
    print("\n" + validator.generate_report())
    
    # Return exit code based on errors
    errors = [r for r in validator.results if r.level == ValidationLevel.ERROR]
    return 0 if not errors else 1


if __name__ == "__main__":
    sys.exit(main())
