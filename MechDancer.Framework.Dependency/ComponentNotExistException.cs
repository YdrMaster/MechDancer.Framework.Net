using System;

namespace MechDancer.Framework.Dependency {
	/// <summary>
	/// 	组件不存在异常
	/// </summary>
	public sealed class ComponentNotExistException : Exception {
		/// <summary>
		/// 	构造器
		/// </summary>
		/// <param name="type">不存在的组件类型</param>
		public ComponentNotExistException(Type type)
			: base($"cannot find this dependency: {type.Name}") { }
	}
}