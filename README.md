Changelog
Version 0.6
New Features
Added Colony Services framework.
Added Clean Colony service.
New Clean Colony button in the Merchant Terminal.
Instantly removes all filth from the current colony, including:
Blood
Dirt
Trash
Ash
Vomit
Other filth types
Service now charges silver before cleaning the colony.
Settings
Added Enable Colony Services setting.
Added Enable Clean Colony setting.
Added configurable Clean Colony Cost.
Improvements
Clean Colony service now verifies the colony has enough silver before performing the service.
Silver is deducted correctly even when spread across multiple stacks.
Service displays a confirmation message showing how many filth objects were removed.
Bug Fixes
Fixed a crash caused by modifying the silver collection while iterating through it (InvalidOperationException: Collection was modified).
Improved stability when deducting silver from multiple stacks.
