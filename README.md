UniEasy - simple framework for unity
===
Created by chaolun

welcome, this framework is based on my years of project experience summed up.
i hope everyone like it and easy to use,cheers!

- <a href="#introduction">Introduction</a>
- <a href="#quick_start">Quick Start</a>
- <a href="#history">History</a>

## <a id="introduction"></a>Introduction
...

## <a id="quick_start"></a>Quick Start
...

## <a id="history"></a>History
2016-09-08 Create A Github Project
	
1.set user name and email

	$ git config --global user.name "humingx"
  	$ git config --global user.email "humingx@yeah.net"
	
2.create ssh key

	$ ssh-keygen -t rsa -C "humingx@yeah.net"
	if don't need password continuous input enter 3 times
	finally,get 2 files: id_rsa and id_rsa.pub
	
3.login Github and add ssh id_rsa.pub

	copy id_rsa.pub content to Settings>>SSH and GPG keys>>New SSH key
	Title can input authorized_keys
	
4.test ssh whether successful

	$ ssh -T git@github.com
	if you see :
		The authenticity of host 'github.com (207.97.227.239)' can't be established.
   	 	RSA key fingerprint is 16:27:ac:a5:76:28:2d:36:63:1b:56:4d:eb:df:a6:48.
    		Are you sure you want to continue connecting (yes/no)?
	select: yes
	if you see your user name behind Hi,success!
	
5.modified .git/config file

	url = https://github.com/...
	change to
	url = git@github.com/...

2016-10-22 Create Unity Project ver5.4.1f1

2016-11-26 Create Submodule

	1.remove uniRx plugin ver5.5.0
	2.create easy plugins repository
	3.git submodule add  git@github.com:chaolunner/EasyPlugins.git Assets/Plugins
	4.add uniRx plugin to Plugins folder

2016-12-12 EasyWriter

	1.Now you can save Primitive,Struct,Class,MonoBehavior,ScriptableObject or their Array
	2.First used EasyWriter writer = new EasyWriter (filePath) Create and Save the json file in filePath
	3.Then Call Set<T> (string key, T value) and SetArray<T> (string key, object value) to save the data
	4.Call Get<T> (string key) and GetArray<T> (string key) to load the data
	5.Call Get<T> (string key, T target) and GetArray<T> (string key, T[] target) to overwrite the MonoBehavior or ScriptableObject

2017-03-18 Dependency Injection

	0.Before Start : Don't forgot using UniEasy.DI!
	1.About Inject - You can use like : 

		1 - Constructor Injection
	
			public class Foo 
			{
		    	    IBar bar;

		    	    public Foo(IBar bar)
		    	    {
			        this.bar = bar;
		    	    }
			}
		
		2 - Field Injection
		
			public class Foo
			{
			    [Inject]
			    IBar bar;
			}
		
		3 - Property Injection
		
			public class Foo
			{
		    	    [Inject]
		    	    public IBar Bar {
			        get;
			        private set;
			    }
			}
		
		4 - Method Injection
		
			public class Foo
			{
			    IBar bar;

			    [Inject]
			    public Init(IBar bar)
			    {
			        this.bar = bar;
			    }
			}
		
	2.About Binding - You can use like : 
	
		Container.Bind<Foo>().AsSingle();
		Container.Bind<IBar>().To<Bar>().AsSingle();
		Container.Bind<IBar>().To<Bar>().FromInstance (new Bar ()).AsSingle();
		Container.Bind<IBar> ().WithId ("id").To<Bar> ().FromInstance (new Bar ()).AsSingle ();
		Container.Bind<Foo>().AsSingle().WhenInjectedInto<Bar> (); -- (It mean delay binding wait until have Bar type Inject).
		Container.Bind<Foo>().AsSingle().NonLazy (); -- (Normally, the ResultType is only ever instantiated when the binding is first used. However, when NonLazy is used, ResultType will immediately by created on startup).
		
	3.Want Inject work - after var bar = new Bar() don't forgot use DiContainer.Inject (bar); if class Bar is sub class of MonoBehaviour, it is better add DiContainer.Inject (this) to void Awake ().

2017-03-19 Ranged Int & Ranged Float

	you can use RangedInt or RangedFloat like : 
		public class Example {
		    [MinMaxRange (0, 10)]
		    public RangedInt a;
		    [MinMaxRange (0, 10)]
		    public RangedFloat b;
		}
	then you just need drag the slider to select the range in Inspector

2017-03-19 ScriptableObject Window

	1. Right click in Project Window Select >> Create >> UniEasy >> ScriptableObject Window.
	2. You can use search or drop-down menu select a ScriptableObject class (you can found all ScriptableObject class in project).
	3. Click Create Button create you selected class as a asset file.
	
2017-03-19 Template Script Window

	1. Right click in Project Window Select >> Create >> UniEasy >> Template Script Window.
	2. You can use search or drop-down menu select a IScriptAssetInstaller (you can found all IScriptAssetInstaller in project).
	3. Click Create Button create you selected template script as a .cs file.
	
2017-03-19 Entity-Component-System

	1. As a Entity you need add a EntityBehaviour Component on GameObject.
	2. You can use Right click in Project Window Select >> Create >> UniEasy >> ComponentBehaviour Installer Create a 'Component'.
	3. You can use Right clicj in Project Window Select >> Create >> UniEasy >> SystemBehaviour Installer Create a 'System'.
	4. For Example : 

[Entity]

	Scene : 
		Entity(GameObject) : 
			.Transform
			.EntityBehaviour
			.Health(ComponentBehaviour)
	DontDestoryOnLoadScene : 
		System(GameObject) : 
			.Transform
			.HealthSystem

[Component]

	using UnityEngine;
	using UniEasy.ECS;
	using UniEasy;
	using System;
	using UniRx;

	public class Health : ComponentBehaviour
	{
  	    public float CurrentHealth;
	    public float StartingHealth;
	    
	    protected override void Awake ()
	    {
	        base.Awake ();
	    }
	    
	    void Start ()
	    {
	    }
	}
	
[System]

	using UnityEngine;
	using UniEasy.ECS;
	using UniEasy;
	using System;
	using UniRx;
	
	public class HealthSystem : SystemBehaviour
	{
	    protected override void Awake ()
	    {
	        base.Awake ();
	    }
	    
  	    void Start()
	    {
		var HealthComponents = GroupFactory.Create(typeof(Health));
		HealthComponents.Entities.ObserveAdd ().Select(x => x.Value).StartWith(group.Entities).Subscribe (entity =>
        	{
          	    var healthComponent = entity.GetComponent<Health>()
         	    healthComponent.CurrentHealth = healthComponent.StartingHealth;
        	}).AddTo(this.Disposer);
  	    }
	}

2017-03-19 Debug System

	using UniEasy;
	
	public class Example : MonoBehaviour
	{
	    void Start ()
	    {
	        Debugger.Log ("white context", "Layer 0");
		Debugger.LogWarnning ("yellow context", "Layer 1");
		Debugger.LogError ("red context", "Layer 2");
	    }
	}
	
1. Run in editor found DebugSystem Component at DontDestoryOnLoad Scene >> DebugSystem GameObject >> DebugSystem Component.
2. you can see a IsLogEnable toggle on DebugSystem Component at Inspector Window, if IsLogEnable == false nothing on Console Window.
3. you can see all layers toggle on DebugSystem Component at Inspector Window, if layer0 == false "white context" will not output to Console Window.
4. the new layer will auto add to debugsystem when you debugger.log ("", "a new layer name") when run in editor;

2017-03-26 Context Menu
	
Now you can use EasyMenuItem Attribute Instead of MenuItem Attribute, its usage is very similar to MenuItem. But why not use MenuItem, because I don't like the sort of MenuItem, it can't adjust the order of the parent menu by setting the priority value.
Use EasyMenuItem the function you want to called must isStatic, you can set the partition line by setting the priority value (set up a split line for every 50 items), you can even call a method with a parameter and parameter type must be set to System.Object, the incoming parameter is your selected Object.
For example, you can call hierarchy context menu like this : 

	[EasyMenuItem ("GameObject/Do Something/NonParam", false, 1)]
	static public void DoSomething ()
	{
	    ...
	}
	
	[EasyMenuItem ("GameObject/Do Something/NonParam", true, 1)]
	static public bool CheckDoSomething ()
	{
	    ...
	    return true;
	}
	
	[EasyMenuItem ("GameObject/Do Something/Param", false, 2)]
	static public void DoSomething (object activeGameObject)
	{
	    ...
	}
	
2017-03-26 Search Missing Component

The component on the GameObject is missing ? This is a very common situation, because programmers need to modify the component and can not remember how many GameObjects add the component. Of course, we don't want to missing the components of the GameObject exists in the scene. We can code a simple function to remove all missing components, but it's not intuitive, maybe we need know where missing component and use a new component to replace it.

Fortunately, UniEasy achieved this feature. you can right click in hierarchy window and select >> UniEasy >> Search for All Missing Components then click. Now All game objects that missing component are listed in the hierarchy window.

2017-04-08 Upgrade Project to Unity5.6.0f3

2017-04-08 Add Distortion Effect

	You can add this component to the Text(UGUI), then you can adjust the animation curve in the Inspector window to change the shape of the Text's text.
	You also can add it use Inspector >> Add Component >> UI >> Effects >> Distortion.

~~2017-04-09 Operate Active Entity~~
> We don't use it any more (2017/05/14) <a href="#20170514">Use this replace</a>

	You can use GroupFactory.CreateAsSingle() replace GroupFactory.Create() most of the time. This will help to improve performance.
	
	public class SystemBehaviour {
	    var group = GroupFactory.CreateAsSingle (new Type[] {
	        typeof(EntityBehaviour),
	    });
	    
	    // it will debug when a entity be set active = true or entitybehaviour awake.
	    // you can use group.GetEntities (true) get all active entities,
	    // use group.GetEntities (false) get all nonactive entities.
	    group.GetEntities (true).ObserveAdd ().Select (x => x.Value).StartWith (group.Entities).Subscribe (entity => {
	        Debugger.Log ("", "UniEasy");
	    }).AddTo (this.Disposer);
	    
	    // it will debug when a entity be set active = false or destroy.
	    group.GetEntities (true).ObserveRemove ().Select (x => x.Value).Subscribe (entity => {
	        Debugger.Log ("", "UniEasy");
	    }).AddTo (this.Disposer);
	}

2017-05-06 Scene Container

        Separate Inject Container to Project Container and Scene Container. The Project Container is a singleton, 
	global, you can call through UniEasyInstaller.ProjectContainer.
	
        Every scene has a Scene Container expect DontDestroyOnLoad scene. Them are Project Container's childs, 
	so you can get Project Container bindings on them.
	
        Quick start, you just need to add the SceneInstaller component to the root gameobject in the scene. 
	SceneInstaller will binding all systems(SystemBehaviour) in this scene. All ComponentBehaviour and SystemBehaviour 
	will be auto inject, These logic codes are written on the UniEasyInstaller.
	
        Why we need separate container? Because the script call between the scene and the scene will increase the coupling. 
	If it is core data you can use Project Container bind it. If is not, it should only can be called in this scene. 
	If other scene need to its data you can use EventSystem transfer data.
	
<a id="20170514"></a>2017-05-14 GroupFactory WithPredicate

>###### Now we add 'with preficate' function to the group, so we can do more cool things. And it can completely replaced 'Operate Active Entity', like this : 
	
	var group = GroupFactory
		.AddTypes (new Type[] { typeof(EntityBehaviour), typeof(ActiveComponent) })
		.WithPredicate ((entity) => {
			var activeComponent = e.GetComponent<ActiveComponent> ();
			activeComponent.gameObject.ObserveEveryValueChanged (go => go.activeSelf).Subscribe (b => {
				activeComponent.IsActiveSelf.Value = b;
			}).AddTo (activeComponent.Disposer);
			return activeComponent.IsActiveSelf;
		}).Create ();
		
	group.Entities.ObserveAdd ().Select (x => x.Value).StartWith (group.Entities).Subscribe (entity => {
		Debug.Log ("each time the gameObject is set to active will be called");
	}).AddTo (this.Disposer);

	group.Entities.ObserveRemove ().Select (x => x.Value).Subscribe (entity => {
		Debug.Log ("each time the gameObject is set to inactive will be called");
	}).AddTo (this.Disposer);
	
>##### Why we remove GroupFactory.CreateAsSingle() ? 
> because in future group should be disposed depened on system and after add 'with preficate', create a single group it will become more complex and unstable. As a replacement for it, for a high frequency group you can choose to create a class for him to inherit group, then bind and inject it into every system that needs to be used.
