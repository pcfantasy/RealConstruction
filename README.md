**Read Me File for Real Construction mod**
===

*Mod folder lits:*
---

* .vs - contain the extentions in which use to the access the project on Windows Visual Studio application

* bin - self explainatory

* Custom AI - constains modified cs files in which change the behavior of the game AI mechanic. Ranging from citizens AI to the vehicles AI

* Custom Manger - constains modified cs files in change the game behavior from user perspectives. UI interactions, options, etc

* Icon - Stored icon pictures in which the mod uses

* Locales - Stored any localization works which will translate the game into a pre-written languages

* obj - store any references libraries or assemblies which the mod will need to function

* Properties - store the cs file assembly file

* UI - store cs files defining the User Interface within the game

* Util - sto cs helper files


*Mod file lits:*
---

* Loader - provide the game instruction of how to load the mod

* RealConstruction - provide the game interactions instruction

* RealConstruction/Project - allow access into the mod entire Visual Studio Project

* RealConstruction/Solution - provide any source file or relavent data

* RealConstructionThreading - provide instruction for cpu threading usage


*Author and collaborators:*
---

Author: **pcfantasy**

Collaborators: **Pourov(Sagiluv1)**


*Update logs:*
---

* 20/03/2019;
	1. Increase resource building production speed a little
	2. Add garbage sevice as high priority in resource supply
	3. Add a option to disable lack of resource icon
	4. Replace Building UI botton with pictures
	5. Update translation system with en.tx and zh.txt (You can add other languages for me by github)
		
* 11/03/2019;
	1. Update priority system:	
	2. Water and electricity building will get resource first.
	3. Discuss this update in the discussions section.
		
* 11/03/2019;
	1. Change mod name
	2. Optimize runtime in transfermanager
	3. Fix new matchoffer logic issue.
		
* 10/03/2019;
	1. Hotfix: New version may have some bugs, revert to old version
		
* 10/03/2019;
	1. Can not transfer resource to parkbuilding, fix this issue.
	2. Fix a Object reference not set to an instance of an object Error
	3. Resource building can use more trucks now.
	4. Auto change language
		
* 10/03/2019;
	1. Increase city resource building production speed.
	2. Increase operation material buffer for playerbuilding
	3. Fix a realconstruction UI issue in buildingUI
	4. Optimize code for runtime
	5. Optimize matchoffer logic to make the shortest transfer for resource.
		
* 06/03/2019;
	1. Fix a error report after close current game.
	2. Optimize matchoffer code
		
* 22/02/2019;
	1. Remove Dependency of RealGasStation Mod
	2. Add check detour logic
	3. Code restruct for RealCity Mod
		
* 10/02/2019;
	1. Remove workaroud logic for blow realtime bug
	2. RealTime Bug fix
	3. Hotfix 1.17.2 - a little construction bug

	* Fixed a bug: demolishing a constructing building (residential, commercial, industrial, office) caused the construction limit to decrease
		
* 07/02/2019;
	1. Can not transfer resource to parkbuilding, fix this issue.
	2. Fix a Object reference not set to an instance of an object Error
	3. Resource building can use more trucks now.
	4. Auto change language
