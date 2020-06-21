### Lore
- ambient air is saturated with light
- weapon (miracell) stores light for orb attacks
- when below full capacity, it attempts to reload by draining ambient light
- when at zero capacity, light hunger increases and it can absorb light fast enough to channel it into a beam
- alternatively, weapon can overcharge and release all its stored light at once

### Input Flow
- orb attack
	- clip emptied
		- shoot beam
			- hold beam
			- let go to drain light
		- wait to drain light
	- orbs remaining
		- charge attack
			- clip empties
		- orb attack again
		- wait to drain light
