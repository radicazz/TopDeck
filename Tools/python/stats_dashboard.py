#!/usr/bin/env python3
"""
Stats Dashboard for TopDeck Unity Game
Real-time gameplay statistics viewer and analyzer.
Tracks performance metrics, game balance, and player progress.
"""

import json
import time
import datetime
from typing import Dict, List, Any, Optional, Tuple
from dataclasses import dataclass, field
from enum import Enum
from collections import deque, defaultdict
import math


class StatCategory(Enum):
    """Categories of statistics"""
    COMBAT = "Combat"
    ECONOMY = "Economy"
    WAVES = "Waves"
    UPGRADES = "Upgrades"
    PERFORMANCE = "Performance"
    DEFENDERS = "Defenders"
    ENEMIES = "Enemies"
    SESSION = "Session"


@dataclass
class StatEntry:
    """Single statistics entry"""
    timestamp: float
    category: StatCategory
    name: str
    value: Any
    unit: str = ""


@dataclass 
class GameSession:
    """Tracks a complete game session"""
    start_time: float
    end_time: Optional[float] = None
    waves_completed: int = 0
    enemies_killed: int = 0
    defenders_lost: int = 0
    money_earned: int = 0
    money_spent: int = 0
    upgrades_purchased: int = 0
    highest_wave: int = 0
    total_damage_dealt: float = 0.0
    total_damage_taken: float = 0.0
    session_id: str = ""
    
    def duration(self) -> float:
        """Get session duration in seconds"""
        end = self.end_time or time.time()
        return end - self.start_time


class StatsDashboard:
    """Real-time statistics dashboard for TopDeck"""
    
    def __init__(self, history_size: int = 1000):
        self.stats_history: deque = deque(maxlen=history_size)
        self.current_session: Optional[GameSession] = None
        self.sessions: List[GameSession] = []
        
        # Real-time counters
        self.counters = defaultdict(lambda: defaultdict(float))
        
        # Performance metrics
        self.frame_times: deque = deque(maxlen=100)
        self.fps_history: deque = deque(maxlen=300)
        
        # Wave statistics
        self.wave_stats = {
            "current_wave": 0,
            "wave_start_time": 0,
            "enemies_this_wave": 0,
            "enemies_killed_this_wave": 0,
            "wave_duration": 0,
            "wave_efficiency": 0.0
        }
        
        # Combat statistics  
        self.combat_stats = {
            "kills_per_minute": 0.0,
            "damage_per_second": 0.0,
            "accuracy": 0.0,
            "shots_fired": 0,
            "shots_hit": 0,
            "critical_hits": 0,
            "overkill_damage": 0.0
        }
        
        # Economy statistics
        self.economy_stats = {
            "current_money": 0,
            "income_rate": 0.0,
            "spend_rate": 0.0,
            "efficiency_ratio": 0.0,
            "cost_per_kill": 0.0
        }
        
        # Upgrade statistics
        self.upgrade_stats = {
            "total_upgrades": 0,
            "upgrade_levels": {},
            "upgrade_efficiency": {},
            "most_upgraded": None
        }
    
    def start_session(self, session_id: str = None) -> GameSession:
        """Start a new game session"""
        if self.current_session and not self.current_session.end_time:
            self.end_session()
        
        self.current_session = GameSession(
            start_time=time.time(),
            session_id=session_id or datetime.datetime.now().strftime("%Y%m%d_%H%M%S")
        )
        
        self.record_stat(StatCategory.SESSION, "session_started", 1)
        return self.current_session
    
    def end_session(self) -> Optional[GameSession]:
        """End the current game session"""
        if not self.current_session:
            return None
        
        self.current_session.end_time = time.time()
        self.sessions.append(self.current_session)
        
        self.record_stat(StatCategory.SESSION, "session_ended", 1)
        session = self.current_session
        self.current_session = None
        return session
    
    def record_stat(self, category: StatCategory, name: str, value: Any, unit: str = ""):
        """Record a statistics entry"""
        entry = StatEntry(
            timestamp=time.time(),
            category=category,
            name=name,
            value=value,
            unit=unit
        )
        
        self.stats_history.append(entry)
        self.counters[category][name] = value
        
        # Update session stats if active
        if self.current_session:
            self._update_session_stats(category, name, value)
    
    def _update_session_stats(self, category: StatCategory, name: str, value: Any):
        """Update current session statistics"""
        if not self.current_session:
            return
        
        if category == StatCategory.WAVES:
            if name == "wave_completed":
                self.current_session.waves_completed += 1
                self.current_session.highest_wave = max(
                    self.current_session.highest_wave, value
                )
        elif category == StatCategory.COMBAT:
            if name == "enemy_killed":
                self.current_session.enemies_killed += 1
            elif name == "damage_dealt":
                self.current_session.total_damage_dealt += value
            elif name == "damage_taken":
                self.current_session.total_damage_taken += value
        elif category == StatCategory.DEFENDERS:
            if name == "defender_lost":
                self.current_session.defenders_lost += 1
        elif category == StatCategory.ECONOMY:
            if name == "money_earned":
                self.current_session.money_earned += value
            elif name == "money_spent":
                self.current_session.money_spent += value
        elif category == StatCategory.UPGRADES:
            if name == "upgrade_purchased":
                self.current_session.upgrades_purchased += 1
    
    def update_wave_stats(self, wave_number: int, enemies_spawned: int = 0):
        """Update wave-related statistics"""
        self.wave_stats["current_wave"] = wave_number
        self.wave_stats["wave_start_time"] = time.time()
        self.wave_stats["enemies_this_wave"] = enemies_spawned
        self.wave_stats["enemies_killed_this_wave"] = 0
        
        self.record_stat(StatCategory.WAVES, "wave_started", wave_number)
    
    def complete_wave(self):
        """Mark current wave as completed"""
        if self.wave_stats["wave_start_time"] > 0:
            duration = time.time() - self.wave_stats["wave_start_time"]
            self.wave_stats["wave_duration"] = duration
            
            # Calculate efficiency
            if self.wave_stats["enemies_this_wave"] > 0:
                self.wave_stats["wave_efficiency"] = (
                    self.wave_stats["enemies_killed_this_wave"] / 
                    self.wave_stats["enemies_this_wave"]
                )
            
            self.record_stat(StatCategory.WAVES, "wave_completed", 
                           self.wave_stats["current_wave"])
            self.record_stat(StatCategory.WAVES, "wave_duration", duration, "seconds")
            self.record_stat(StatCategory.WAVES, "wave_efficiency", 
                           self.wave_stats["wave_efficiency"] * 100, "%")
    
    def update_combat_stats(self, shots_fired: int = 0, shots_hit: int = 0,
                           damage_dealt: float = 0, critical: bool = False):
        """Update combat-related statistics"""
        self.combat_stats["shots_fired"] += shots_fired
        self.combat_stats["shots_hit"] += shots_hit
        
        if critical:
            self.combat_stats["critical_hits"] += 1
        
        if shots_fired > 0:
            self.combat_stats["accuracy"] = (
                self.combat_stats["shots_hit"] / self.combat_stats["shots_fired"]
            ) * 100
        
        self.record_stat(StatCategory.COMBAT, "damage_dealt", damage_dealt)
        self.record_stat(StatCategory.COMBAT, "accuracy", 
                        self.combat_stats["accuracy"], "%")
    
    def update_economy_stats(self, current_money: int, earned: int = 0, spent: int = 0):
        """Update economy-related statistics"""
        self.economy_stats["current_money"] = current_money
        
        if earned > 0:
            self.record_stat(StatCategory.ECONOMY, "money_earned", earned)
        if spent > 0:
            self.record_stat(StatCategory.ECONOMY, "money_spent", spent)
        
        # Calculate rates
        if self.current_session:
            duration = self.current_session.duration() / 60.0  # Convert to minutes
            if duration > 0:
                self.economy_stats["income_rate"] = (
                    self.current_session.money_earned / duration
                )
                self.economy_stats["spend_rate"] = (
                    self.current_session.money_spent / duration
                )
                
                if self.current_session.money_earned > 0:
                    self.economy_stats["efficiency_ratio"] = (
                        self.current_session.money_spent / 
                        self.current_session.money_earned
                    )
                
                if self.current_session.enemies_killed > 0:
                    self.economy_stats["cost_per_kill"] = (
                        self.current_session.money_spent / 
                        self.current_session.enemies_killed
                    )
    
    def update_upgrade_stats(self, defender_type: str, level: int, cost: int):
        """Update upgrade-related statistics"""
        self.upgrade_stats["total_upgrades"] += 1
        self.upgrade_stats["upgrade_levels"][defender_type] = level
        
        # Track efficiency (damage increase per cost)
        if defender_type not in self.upgrade_stats["upgrade_efficiency"]:
            self.upgrade_stats["upgrade_efficiency"][defender_type] = {
                "total_cost": 0,
                "level": 0,
                "efficiency": 0.0
            }
        
        self.upgrade_stats["upgrade_efficiency"][defender_type]["total_cost"] += cost
        self.upgrade_stats["upgrade_efficiency"][defender_type]["level"] = level
        
        # Find most upgraded defender
        max_level = 0
        most_upgraded = None
        for def_type, lvl in self.upgrade_stats["upgrade_levels"].items():
            if lvl > max_level:
                max_level = lvl
                most_upgraded = def_type
        
        self.upgrade_stats["most_upgraded"] = most_upgraded
        
        self.record_stat(StatCategory.UPGRADES, "upgrade_purchased", 1)
        self.record_stat(StatCategory.UPGRADES, f"{defender_type}_level", level)
    
    def update_fps(self, delta_time: float):
        """Update FPS statistics"""
        self.frame_times.append(delta_time)
        
        if delta_time > 0:
            fps = 1.0 / delta_time
            self.fps_history.append(fps)
            
            # Calculate averages
            avg_fps = sum(self.fps_history) / len(self.fps_history)
            min_fps = min(self.fps_history) if self.fps_history else 0
            max_fps = max(self.fps_history) if self.fps_history else 0
            
            self.record_stat(StatCategory.PERFORMANCE, "current_fps", fps)
            self.record_stat(StatCategory.PERFORMANCE, "average_fps", avg_fps)
            self.record_stat(StatCategory.PERFORMANCE, "min_fps", min_fps)
            self.record_stat(StatCategory.PERFORMANCE, "max_fps", max_fps)
    
    def get_summary(self) -> Dict[str, Any]:
        """Get a summary of all current statistics"""
        summary = {
            "session": {},
            "waves": self.wave_stats,
            "combat": self.combat_stats,
            "economy": self.economy_stats,
            "upgrades": self.upgrade_stats,
            "performance": {}
        }
        
        # Session summary
        if self.current_session:
            summary["session"] = {
                "id": self.current_session.session_id,
                "duration": self.current_session.duration(),
                "waves_completed": self.current_session.waves_completed,
                "enemies_killed": self.current_session.enemies_killed,
                "defenders_lost": self.current_session.defenders_lost,
                "kd_ratio": (self.current_session.enemies_killed / 
                           max(1, self.current_session.defenders_lost))
            }
        
        # Performance summary
        if self.fps_history:
            summary["performance"] = {
                "current_fps": self.fps_history[-1] if self.fps_history else 0,
                "average_fps": sum(self.fps_history) / len(self.fps_history),
                "min_fps": min(self.fps_history),
                "max_fps": max(self.fps_history),
                "stable": max(self.fps_history) - min(self.fps_history) < 20
            }
        
        # Calculate derived metrics
        if self.current_session:
            duration_minutes = self.current_session.duration() / 60.0
            if duration_minutes > 0:
                summary["combat"]["kills_per_minute"] = (
                    self.current_session.enemies_killed / duration_minutes
                )
                summary["combat"]["damage_per_second"] = (
                    self.current_session.total_damage_dealt / 
                    self.current_session.duration()
                )
        
        return summary
    
    def get_recent_stats(self, category: Optional[StatCategory] = None, 
                        limit: int = 100) -> List[StatEntry]:
        """Get recent statistics entries"""
        if category:
            filtered = [s for s in self.stats_history if s.category == category]
            return list(filtered)[-limit:]
        return list(self.stats_history)[-limit:]
    
    def generate_report(self) -> str:
        """Generate a formatted statistics report"""
        summary = self.get_summary()
        
        report = ["=" * 60]
        report.append("TOPDECK STATISTICS DASHBOARD")
        report.append("=" * 60)
        report.append("")
        
        # Session info
        if summary["session"]:
            report.append("📊 CURRENT SESSION")
            report.append("-" * 40)
            sess = summary["session"]
            duration = sess["duration"]
            hours = int(duration // 3600)
            minutes = int((duration % 3600) // 60)
            seconds = int(duration % 60)
            
            report.append(f"  Session ID: {sess['id']}")
            report.append(f"  Duration: {hours:02d}:{minutes:02d}:{seconds:02d}")
            report.append(f"  Waves Completed: {sess['waves_completed']}")
            report.append(f"  Enemies Killed: {sess['enemies_killed']}")
            report.append(f"  Defenders Lost: {sess['defenders_lost']}")
            report.append(f"  K/D Ratio: {sess['kd_ratio']:.2f}")
            report.append("")
        
        # Wave stats
        report.append("🌊 WAVE STATISTICS")
        report.append("-" * 40)
        wave = summary["waves"]
        report.append(f"  Current Wave: {wave['current_wave']}")
        report.append(f"  Enemies This Wave: {wave['enemies_this_wave']}")
        report.append(f"  Enemies Killed: {wave['enemies_killed_this_wave']}")
        report.append(f"  Wave Efficiency: {wave['wave_efficiency']*100:.1f}%")
        if wave['wave_duration'] > 0:
            report.append(f"  Last Wave Duration: {wave['wave_duration']:.1f}s")
        report.append("")
        
        # Combat stats
        report.append("⚔️ COMBAT STATISTICS")
        report.append("-" * 40)
        combat = summary["combat"]
        report.append(f"  Accuracy: {combat['accuracy']:.1f}%")
        report.append(f"  Shots Fired: {combat['shots_fired']:,}")
        report.append(f"  Shots Hit: {combat['shots_hit']:,}")
        report.append(f"  Critical Hits: {combat['critical_hits']:,}")
        report.append(f"  Kills/Minute: {combat.get('kills_per_minute', 0):.1f}")
        report.append(f"  DPS: {combat.get('damage_per_second', 0):.1f}")
        report.append("")
        
        # Economy stats
        report.append("💰 ECONOMY STATISTICS")
        report.append("-" * 40)
        econ = summary["economy"]
        report.append(f"  Current Money: ${econ['current_money']:,}")
        report.append(f"  Income Rate: ${econ['income_rate']:.1f}/min")
        report.append(f"  Spend Rate: ${econ['spend_rate']:.1f}/min")
        report.append(f"  Efficiency: {econ['efficiency_ratio']*100:.1f}%")
        report.append(f"  Cost per Kill: ${econ['cost_per_kill']:.2f}")
        report.append("")
        
        # Upgrade stats
        report.append("⬆️ UPGRADE STATISTICS")
        report.append("-" * 40)
        upgr = summary["upgrades"]
        report.append(f"  Total Upgrades: {upgr['total_upgrades']}")
        report.append(f"  Most Upgraded: {upgr['most_upgraded'] or 'None'}")
        
        if upgr['upgrade_levels']:
            report.append("  Current Levels:")
            for def_type, level in upgr['upgrade_levels'].items():
                report.append(f"    {def_type}: Level {level}")
        report.append("")
        
        # Performance stats
        if summary["performance"]:
            report.append("⚡ PERFORMANCE STATISTICS")
            report.append("-" * 40)
            perf = summary["performance"]
            report.append(f"  Current FPS: {perf['current_fps']:.0f}")
            report.append(f"  Average FPS: {perf['average_fps']:.0f}")
            report.append(f"  Min FPS: {perf['min_fps']:.0f}")
            report.append(f"  Max FPS: {perf['max_fps']:.0f}")
            stability = "✅ Stable" if perf['stable'] else "⚠️ Unstable"
            report.append(f"  Stability: {stability}")
            report.append("")
        
        report.append("=" * 60)
        
        return "\n".join(report)
    
    def export_session_data(self, session: Optional[GameSession] = None) -> Dict:
        """Export session data for analysis"""
        if not session:
            session = self.current_session
        
        if not session:
            return {}
        
        return {
            "session_id": session.session_id,
            "start_time": session.start_time,
            "end_time": session.end_time,
            "duration": session.duration(),
            "waves_completed": session.waves_completed,
            "highest_wave": session.highest_wave,
            "enemies_killed": session.enemies_killed,
            "defenders_lost": session.defenders_lost,
            "money_earned": session.money_earned,
            "money_spent": session.money_spent,
            "upgrades_purchased": session.upgrades_purchased,
            "total_damage_dealt": session.total_damage_dealt,
            "total_damage_taken": session.total_damage_taken,
            "efficiency_metrics": {
                "kills_per_wave": (session.enemies_killed / 
                                  max(1, session.waves_completed)),
                "money_efficiency": (session.money_spent / 
                                   max(1, session.money_earned)),
                "damage_ratio": (session.total_damage_dealt / 
                               max(1, session.total_damage_taken))
            }
        }
    
    def get_analytics(self) -> Dict[str, Any]:
        """Generate analytics insights from statistics"""
        analytics = {
            "recommendations": [],
            "warnings": [],
            "achievements": []
        }
        
        summary = self.get_summary()
        
        # Performance analysis
        if summary.get("performance"):
            avg_fps = summary["performance"]["average_fps"]
            if avg_fps < 30:
                analytics["warnings"].append("⚠️ Low FPS detected - consider reducing quality settings")
            elif avg_fps > 60:
                analytics["achievements"].append("🏆 Excellent performance - running smoothly!")
        
        # Combat analysis
        if summary["combat"]["accuracy"] < 50:
            analytics["recommendations"].append("💡 Low accuracy - try focusing fire on single targets")
        elif summary["combat"]["accuracy"] > 80:
            analytics["achievements"].append("🎯 Sharp shooter - excellent accuracy!")
        
        # Economy analysis
        econ = summary["economy"]
        if econ["efficiency_ratio"] > 0.9:
            analytics["warnings"].append("⚠️ Spending nearly all income - save for emergencies")
        elif econ["efficiency_ratio"] < 0.5:
            analytics["recommendations"].append("💡 Under-spending - consider more upgrades")
        
        # Wave progress
        if self.current_session:
            if self.current_session.waves_completed > 20:
                analytics["achievements"].append("🌟 Veteran defender - 20+ waves completed!")
            
            kd_ratio = (self.current_session.enemies_killed / 
                       max(1, self.current_session.defenders_lost))
            if kd_ratio > 100:
                analytics["achievements"].append("💪 Unstoppable force - 100:1 K/D ratio!")
        
        return analytics


def main():
    """Demo the statistics dashboard"""
    dashboard = StatsDashboard()
    
    # Start a session
    session = dashboard.start_session("demo_session")
    print("Started session:", session.session_id)
    
    # Simulate some game events
    dashboard.update_wave_stats(1, enemies_spawned=10)
    dashboard.update_combat_stats(shots_fired=50, shots_hit=35, damage_dealt=350)
    dashboard.update_economy_stats(current_money=1000, earned=500)
    dashboard.update_upgrade_stats("BasicDefender", level=2, cost=200)
    
    # Simulate FPS updates
    for i in range(10):
        dashboard.update_fps(0.016 + (i % 3) * 0.002)  # ~60 FPS with variation
    
    # Complete a wave
    dashboard.wave_stats["enemies_killed_this_wave"] = 8
    dashboard.complete_wave()
    
    # Generate and print report
    print("\n" + dashboard.generate_report())
    
    # Get analytics
    analytics = dashboard.get_analytics()
    print("\n📈 ANALYTICS")
    print("-" * 40)
    
    if analytics["achievements"]:
        print("Achievements:")
        for achievement in analytics["achievements"]:
            print(f"  {achievement}")
    
    if analytics["recommendations"]:
        print("\nRecommendations:")
        for rec in analytics["recommendations"]:
            print(f"  {rec}")
    
    if analytics["warnings"]:
        print("\nWarnings:")
        for warning in analytics["warnings"]:
            print(f"  {warning}")
    
    # Export session data
    session_data = dashboard.export_session_data()
    print("\nSession data exported:", json.dumps(session_data, indent=2))
    
    # End session
    dashboard.end_session()
    print("\nSession ended")
    
    return 0


if __name__ == "__main__":
    main()
