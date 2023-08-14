**Read Me File for Real Construction mod**
===

*Mod folder list:*
---

* .vs - contain the extensions in which use to the access the project on Windows Visual Studio application

* bin - self explanatory

* Custom AI - contains modified cs files in which change the behavior of the game AI mechanic. Ranging from citizens AI to the vehicles AI

* Custom Manger - contains modified cs files in change the game behavior from user perspectives. UI interactions, options, etc

* Icon - Stored icon pictures in which the mod uses

* Locales - Stored any localization works which will translate the game into a pre-written language

* obj - store any references libraries or assemblies which the mod will need to function

* Properties - store the cs file assembly file

* UI - store cs files defining the User Interface within the game

* Util - stores cs helper files


*Mod file list:*
---

* Loader - provide the game instruction of how to load the mod

* Real Construction - provide the game interactions instruction

* Real Construction/Project - allow access into the mod entire Visual Studio Project

* Real Construction/Solution - provide any source file or relevant data

* Real Construction Threading - provide instruction for CPU threading usage

*Mod Description:*
---

* Part I:
	This mod major point is to ensure that any facilities; be it residential, commercial, unique, etc. Ensure that each item received materials to be constructed or the item will not be available for use. Also, certain facilities do required materials in order to remain at peak efficient service(s). If they do not, the facility will remain active but will performed assign duty or duties poorly.

* Part II:
	Due to the game engine limitation certain facilities are exempt from the needs of requiring resources for both construction and operation. Majority of this facilities does not require road connection in order to function. Thus, because this mod heavily depends on the roads to deliver resources throughout and about, these facilities are being exempt.

* Part III:
	The mod has tier priority service functions as well. Meaning certain facilities will always get resources priority over others. While some will be required resources at critical needs. An example, a park may get operating resources before a commercial building because its resource level is at zero while the commercial building may have a low amount of resource.

* Part IV:
	The mod requires another mod called resource building. This facility is an absolute must or else the city will not function without it. This facility at default can spawn up to 10 vehicles at a time. It produces both the construction and operation resources to facilitate growth in the village, town, etc. **Do keep in mind the overall Real City mods aim to function under a population of 66,000. Anything higher might begin to negate other aspect of the mods itself.** 
	* Sub Part IV: Tentatively the author will create a function(s), which will dictate if said facility needs to spawn more than ten vehicles in order to accommodate increasing orders that may surpass the default vehicles amount.


*Author and collaborators:*
---

Author: **@pcfantasy**

Collaborators: **@Pourov(Sagiluv1)** **@iwarin123(iwarin123)**

*Update logs:*
---

* 08/05/2023:
	1. German wording has been corrected under @DJKLI supervision.

* 03/05/2023:
	1. Add Spanish, French, Polish, Brazilian Portuguese translation.

* 02/05/2023:
	1. Add German translation de.txt.

* 27/04/2023:
	1. Compatible with Cities Skylines 1.16.1-f2 (Hubs & Transport)
	2. Add Japanese translation ja.txt.

* 20/03/2019:
	1. Increase resource building production speed a little
	2. Add garbage service as high priority in resource supply
	3. Add an option to disable lack of resource icon
	4. Replace Building UI button with pictures
	5. Update translation system with eng.txt and zh.txt (You can add other languages for me by GitHub)
		
* 11/03/2019:
	1. Update priority system:	
	2. Water and electricity building will get resource first.
	3. Discuss this update in the discussions section.
		
* 11/03/2019:
	1. Change mod name
	2. Optimize runtime in transfer manager
	3. Fix new match offer logic issue.
		
* 10/03/2019:
	1. Hotfix: New version may have some bugs, revert to old version
		
* 10/03/2019:
	1. Cannot transfer resource to park building, fix this issue.
	2. Fix an Object reference not set to an instance of an object Error
	3. Resource building can use more trucks now.
	4. Auto change language
		
* 10/03/2019:
	1. Increase city resource building production speed.
	2. Increase operation material buffer for player building
	3. Fix a real construction UI issue in building UI
	4. Optimize code for runtime
	5. Optimize match offer logic to make the shortest transfer for resource.
		
* 06/03/2019:
	1. Fix an error report after close current game.
	2. Optimize match offer code
		
* 22/02/2019:
	1. Remove Dependency of RealGasStation Mod
	2. Add check detour logic
	3. Code reconstruct for Real City Mod
		
* 10/02/2019:
	1. Remove workaround logic for blow Realtime bug
	2. Realtime Bug fix
	3. Hotfix 1.17.2 - a little construction bug

	* Fixed a bug: demolishing a constructing building (residential, commercial, industrial, office) caused the construction limit to decrease
		
* 07/02/2019:
	1. Cannot transfer resource to park building, fix this issue.
	2. Fix an Object reference not set to an instance of an object Error
	3. Resource building can use more trucks now.
	4. Auto change language
