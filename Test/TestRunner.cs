

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace drvrapp {
	public class TestRunner {
		// assembly file (e.g. MyTestAssembly.dll) which should be in the same directory 
		public static void TestDll(string assemblyFile) {
			string symbolsFile = Path.GetFileNameWithoutExtension(assemblyFile) + ".pdb";

			byte[] assemblyBytes = File.ReadAllBytes(assemblyFile);
			byte[] symbolsBytes = File.ReadAllBytes(symbolsFile);

			Assembly assembly = Assembly.Load(assemblyBytes, symbolsBytes);

			foreach (Type type in assembly.GetTypes())
				if (HasAttribute(type, "TestClass"))
					foreach (MethodInfo method in type.GetMethods())
						if (HasAttribute(method, "TestMethod"))
							RunTest(method);
		}

		private static void RunTest(MethodInfo method) {
			Console.WriteLine("------------------------------------------------------------------------------------------");
			Console.Write("Running {0}.{1}...", method.DeclaringType.Name, method.Name);

			if (HasAttribute(method, "Ignore")) {
				Console.WriteLine("IGNORED");
				return;
			}

			try {
				Type testType = method.DeclaringType;
				object typeClassInstance = Activator.CreateInstance(testType);

				RunInitializeOrCleanup(typeClassInstance, "TestInitialize");
				method.Invoke(typeClassInstance, null);
				RunInitializeOrCleanup(typeClassInstance, "TestCleanup");

				Console.WriteLine("PASSED");
			} catch (Exception e) {
				Console.WriteLine();
				Console.WriteLine(e);
			}
		}

		private static void RunInitializeOrCleanup(object instance, string attribute) {
			MethodInfo method = instance.GetType().GetMethods().SingleOrDefault(x => HasAttribute(x, attribute));
			if (method != null)
				method.Invoke(instance, null);
		}

		private static bool HasAttribute(MemberInfo info, string attributeName) {
			return info.GetCustomAttributes(false).Any(x => x.GetType().Name == attributeName + "Attribute");
		}
	}
}



