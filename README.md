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
