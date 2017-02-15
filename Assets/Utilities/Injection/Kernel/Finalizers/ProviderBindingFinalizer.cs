using System.Collections.Generic;
using System;
using System.Linq;

namespace UniEasy
{
	public abstract class ProviderBindingFinalizer : IBindingFinalizer
	{
		private int BindindCount = 0;

		public ProviderBindingFinalizer (BindInfo bindInfo)
		{
			BindInfo = bindInfo;
		}

		protected BindInfo BindInfo {
			get;
			private set;
		}

		public void FinalizeBinding (DiContainer container)
		{
			if (!BindInfo.ContractTypes.Any ()) {
				return;
			}

			OnFinalizeBinding (container);

			if (BindInfo.NonLazy) {
				
			}
		}

		protected abstract void OnFinalizeBinding (DiContainer container);

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
			bool overwrite = BindindCount > 0 ? true : false;
			container.RegisterProvider (
				new BindingId (contractType, BindInfo.Identifier),
				BindInfo.Condition,
				provider, overwrite);

			if (contractType.IsValueType ()) {
				var nullableType = typeof(Nullable<>).MakeGenericType (contractType);

				// Also bind to nullable primitives
				// this is useful so that we can have optional primitive dependencies
				container.RegisterProvider (
					new BindingId (nullableType, BindInfo.Identifier),
					BindInfo.Condition,
					provider, overwrite);
			}
			BindindCount++;
		}
	}
}
