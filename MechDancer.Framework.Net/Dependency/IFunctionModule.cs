namespace MechDancer.Framework.Net.Dependency {
	/// <summary>
	/// 	功能模块
	/// </summary>
	public interface IFunctionModule : IDependency {
		/// <summary>
		/// 	加入动态域
		/// </summary>
		/// <param name="host">目标动态域</param>
		void OnSetup(DynamicScope host);

		/// <summary>
		/// 	重新同步依赖项
		/// </summary>
		void Sync();
	}
}