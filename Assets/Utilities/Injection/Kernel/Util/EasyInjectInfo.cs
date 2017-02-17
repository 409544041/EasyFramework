﻿using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace UniEasy
{
	public class PostInjectableInfo
	{
		readonly MethodInfo methodInfo;
		readonly List<InjectableInfo> injectableInfo;

		public PostInjectableInfo (
			MethodInfo methodInfo, List<InjectableInfo> injectableInfo)
		{
			this.methodInfo = methodInfo;
			this.injectableInfo = injectableInfo;
		}

		public MethodInfo MethodInfo {
			get {
				return methodInfo;
			}
		}

		public IEnumerable<InjectableInfo> InjectableInfo {
			get {
				return injectableInfo;
			}
		}
	}

	public class EasyInjectInfo
	{
		readonly List<InjectableInfo> fieldInjectables;
		readonly List<InjectableInfo> propertyInjectables;
		readonly List<PostInjectableInfo> postInjectMethods;
		readonly List<InjectableInfo> constructorInjectables;

		public EasyInjectInfo (List<InjectableInfo> fieldInjectables,
		                       List<InjectableInfo> propertyInjectables,
		                       List<PostInjectableInfo> postInjectMethods,
		                       List<InjectableInfo> constructorInjectables)
		{
			this.fieldInjectables = fieldInjectables;
			this.propertyInjectables = propertyInjectables;
			this.postInjectMethods = postInjectMethods;
			this.constructorInjectables = constructorInjectables;
		}

		public IEnumerable<InjectableInfo> FieldInjectables {
			get {
				return fieldInjectables;
			}
		}

		public IEnumerable<InjectableInfo> PropertyInjectables {
			get {
				return propertyInjectables;
			}
		}

		public IEnumerable<PostInjectableInfo> PostInjectMethods {
			get {
				return postInjectMethods;
			}
		}

		public IEnumerable<InjectableInfo> ConstructorInjectables {
			get {
				return constructorInjectables;
			}
		}

		public IEnumerable<InjectableInfo> AllInjectables {
			get {
				return constructorInjectables.Concat (fieldInjectables).Concat (propertyInjectables).Concat (postInjectMethods.SelectMany (x => x.InjectableInfo));
			}
		}
	}
}
