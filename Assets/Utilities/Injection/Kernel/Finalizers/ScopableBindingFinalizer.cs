﻿using System.Collections.Generic;
using System;
using System.Linq;

namespace UniEasy
{
	public class ScopableBindingFinalizer : ProviderBindingFinalizer
	{
		readonly Func<DiContainer, Type, IProvider> providerFactory;

		public ScopableBindingFinalizer (
			BindInfo bindInfo,
			Func<DiContainer, Type, IProvider> providerFactory)
			: base (bindInfo)
		{
			this.providerFactory = providerFactory;
		}

		protected override void OnFinalizeBinding (DiContainer container)
		{
			FinalizeBindingConcrete (container, !BindInfo.ToTypes.Any () ? BindInfo.ContractTypes : BindInfo.ToTypes);
		}

		void FinalizeBindingConcrete (DiContainer container, List<Type> concreteTypes)
		{
			if (!concreteTypes.Any ()) {
				return;
			}

			switch (BindInfo.Scope) {
			case ScopeTypes.Singleton:
				RegisterProvidersForAllContractsPerConcreteType (
					container,
					concreteTypes,
					(_, concreteType) => {
						return container.SingletonProviderCreator.GetOrCreateProvider (
							new SingletonId (concreteType, BindInfo.ConcreteIdentifier), 
							providerFactory);
					});
				break;
			default :
				RegisterProvidersForAllContractsPerConcreteType (
					container,
					concreteTypes,
					providerFactory);
				break;
			}
		}
	}
}