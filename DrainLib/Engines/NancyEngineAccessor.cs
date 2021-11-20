using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrainLib.Engines {
	class NancyEngineAccessor : ADBaseEngineAccessor {
		#region Symbol data
		//engine class
		private int gameFlowOffset;

		//gameflow class
		private int curStateOffset;
		private int prevStateOffset;

		//Scene class
		private int sceneStateOffset;

		//SceneState class
		private int currentSceneOffset;

		//SceneInfo class
		private int sceneIDOffset;
		private int frameIDOffset;
		#endregion

		#region Semistatic data
		#endregion

		public NancyEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
		}

		internal override void LoadSymbols() {
			var engineCl = Resolver.FindClass("Nancy::NancyEngine");
			var descriptorOffset = Resolver.FieldOffset(engineCl, "_gameDescription");
			gameFlowOffset = Resolver.FieldOffset(engineCl, "_gameFlow");

			var gfCl = Resolver.FindNestedClass(engineCl,"GameFlow");
			curStateOffset = Resolver.FieldOffset(gfCl, "curState");
			prevStateOffset = Resolver.FieldOffset(gfCl, "prevState");

			var sceneCl = Resolver.FindClass("Nancy::State::Scene");
			sceneStateOffset = Resolver.FieldOffset(sceneCl, "_sceneState");

			var sceneStateCl = Resolver.FindNestedClass(sceneCl, "SceneState");
			currentSceneOffset = Resolver.FieldOffset(sceneStateCl, "currentScene");

			var sceneInfoCl = Resolver.FindClass("Nancy::State::SceneInfo");
			sceneIDOffset = Resolver.FieldOffset(sceneInfoCl, "sceneID");
			frameIDOffset = Resolver.FieldOffset(sceneInfoCl, "frameID");

			LoadADSymbols(descriptorOffset, true);
		}
	}
}
