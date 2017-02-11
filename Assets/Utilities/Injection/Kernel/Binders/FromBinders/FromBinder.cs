﻿using System.Collections.Generic;
using System;
using System.Linq;

namespace UniEasy
{
	public class FromBinder : ScopeBinder
	{
		public FromBinder (BindInfo bindInfo, DiContainer container) : base (bindInfo)
		{
			Container = container;
		}

		public DiContainer Container {
			get;
			private set;
		}

		protected ScopeBinder FromInstanceBase (object instance)
		{
			RegisterProvidersForAllContractsPerConcreteType (
				Container,
				!BindInfo.ToTypes.Any () ? BindInfo.ContractTypes : BindInfo.ToTypes,
				(container, concreteType) => {
					return new InstanceProvider (concreteType, instance);
				});
			return new ScopeBinder (BindInfo);
		}

		// Returns true if the bind should continue, false to skip
		bool ValidateBindTypes (Type concreteType, Type contractType)
		{
			if (concreteType.DerivesFromOrEqual (contractType)) {
				return true;
			}
			return false;
		}

		// Note that if multiple contract types are provided per concrete type,
		// it will re-use the same provider for each contract type
		// (each concrete type will have its own provider though)
		protected void RegisterProvidersForAllContractsPerConcreteType (
			DiContainer container,
			List<Type> concreteTypes,
			Func<DiContainer, Type, IProvider> providerFunc)
		{
			var providerMap = concreteTypes.ToDictionary (x => x, x => providerFunc (container, x));

			foreach (var contractType in BindInfo.ContractTypes) {
				foreach (var concreteType in concreteTypes) {
					if (ValidateBindTypes (concreteType, contractType)) {
						RegisterProvider (container, contractType, providerMap [concreteType]);
					}
				}
			}
		}

		protected void RegisterProvider (DiContainer container, Type contractType, IProvider provider)
		{
			container.RegisterProvider (
				new BindingId (contractType, BindInfo.Identifier),
				BindInfo.Condition,
				provider);

			if (contractType.IsValueType ()) {
				var nullableType = typeof(Nullable<>).MakeGenericType (contractType);

				// Also bind to nullable primitives
				// this is useful so that we can have optional primitive dependencies
				container.RegisterProvider (
					new BindingId (nullableType, BindInfo.Identifier),
					BindInfo.Condition,
					provider);
			}
		}
	}
}
