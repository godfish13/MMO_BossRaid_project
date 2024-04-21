BaseFrameWork
=============
Base FrameWork for UnityEngine Game   

**Contents**
--------
# 1. Plugins
### 1) UniRx   
# 2. Resources
### 1) Art   
##### (1) Material   
##### (2) UnityChan   
### 2) Data   
### 3) Prefabs   
##### (1) Camera   
##### (2) UI   
### 4) Sounds   
##### (1) Bgm   
##### (2) UnityChanVoice   
# 3. Scenes
### 1) InGame
### 2) LogIn
# 4. Scripts
### 1) Controllers   
##### (1) CameraCtrl   
##### (2) PlayerCtrl   
   - Controls Player Character by MousePointer LayCasting   
##### (3) CursorCtrl
   - Show Cursor Icon and changes in various conditions
### 2) Data   
##### (1) DataContents   
   - inherit ILoader in DataMgr and implement individual Data classes   
### 3) Managers   
##### (1) Managers   
   - Main Manager of project whitch init and clear other managers
   - also has other Managers static instance
   - Written as SingleTon pattern
##### -Core folder-   
##### (2) DataMgr
   - ILoader interface is included
   - Load JSON file in path as TextAsset
   - when initted, Saves loaded JSON files as Dictionary
##### (3) InputMgr   
   - Invoke Actions when inputs are entered   
##### (4) PoolMgr   
   - makes Object Pool Stack and use objects by Pop method
   - default object pool size == 5 (if lack of size in pool erupted, class automatically create new object and push it in to pool so that no need to assign number of objects in pool)
###### (4-1) Poolable
   - has no functions but Use as component for judge is Object poolable
##### (5) ResourceMgr   
   - Load/Instantiate/Destroy resources with path   
   - when Instantiate, if object is poolable, get object from objectPool which is made by PoolMgr   
##### (6) SceneMgrEx   
   - includes extended LoadScene method which clear managers when loads other Scene
##### (7) SoundMgr   
##### (8) UIMgr   
### 4) Scenes   
##### (1) BaseScene   
##### (2) InGameScene      
##### (3) LogInScence      
### 5) UI   
* UI   
##### (1) UI_Base   
   - abstract class for other UI classes
   - Bind method which bind UI object in Unity engine with UI enums in classes to use UI as code
##### (2) UI_EventHandler   
#### 5-1) PopUp   
##### (1) UI_Btn   
   - examples of bind UI and use it
##### (2) UI_PopUp   
#### 5-2) Scene   
##### (1) UI_Inven   
##### (2) UI_Scene   
#### 5-3) SubItem   
##### (1) UI_Inven_Item    
### 6) Utility   
##### (1) Define   
   - declare Enums for individual types (Scene, Sound, UIEvent, MouseEvent, CameraMode, ...)   
##### (2) ExtensionMethod   
##### (3) Utils   
   - GetOrAddComponent<T>(GameObject Target) : if Target contains T, call Target's T / if not, add T to Target and call T
   - FindChild<T>(GameObject go, string name = null, bool recursive = false) : Find all T in go's children / if recursive, find all T in go's children and children's children
