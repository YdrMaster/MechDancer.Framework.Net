using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MechDancer.Framework.Net.Dependency;

namespace MechDancer.Framework.Net.Remote.Resources {
	public sealed class Group : IResourceMemory<string, DateTime> {
		private readonly ConcurrentDictionary<string, DateTime> _core
			= new ConcurrentDictionary<string, DateTime>();

		public IReadOnlyDictionary<string, DateTime> View => _core;

		public bool Update(string parameter, DateTime resource, out DateTime previous) {
			DateTime? last = null;
			_core.AddOrUpdate(parameter, _ => resource,
			                  (_, current) => resource.Also(__ => last = current));
			previous = last ?? default;
			return last.HasValue;
		}

		public bool Remove(string parameter) =>
			_core.TryRemove(parameter, out _);

		public bool TryGet(string parameter, out DateTime resource) =>
			_core.TryGetValue(parameter, out resource);

		public DateTime Get(string parameter) =>
			_core.TryGetValue(parameter, out var value) ? value : default;

		public List<string> this[TimeSpan timeout] {
			get {
				var now = DateTime.Now;
				return _core.Where(it => now - it.Value < timeout)
				            .Select(it => it.Key)
				            .ToList();
			}
		}

		public override bool Equals(object obj) => obj is Group;
		public override int  GetHashCode()      => Hash;

		private static readonly int Hash = typeof(Group).GetHashCode();
	}
}