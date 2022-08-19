using System;

namespace DrainLib.Engines {
	public class HadesChEngineAccessor : ADBaseEngineAccessor {

		#region Symbol data
		//engine class
		private int cheatsEnabledOffset;
		private int persistentOffset;
		private int heroBeltOffset;
		private int heroBeltPointerOffset;

		//persistent class
		private int genderOffset;
		private int heroNameOffset;
		private int questOffset;
		private int hintsAreEnabledOffset;
		private int currentRoomIdOffset;
		private int previousRoomIdOffset;
		private const int invenorySize = 6;
		private int inventoryOffset;
		#endregion

		#region Semistatic data
		private IntPtr heroBeltAddr;
		#endregion

		public HadesChEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
			LoadSemiStaticData();
		}

		private void LoadSemiStaticData() {
			heroBeltAddr = MemoryReader.ReadIntPtr(EngineAddr + heroBeltOffset + heroBeltPointerOffset);
		}

		internal override void LoadSymbols() {
			var engineCl = Resolver.FindClass("Hadesch::HadeschEngine");
			var descriptorOffset = Resolver.FieldOffset(engineCl, "_desc");
			cheatsEnabledOffset = Resolver.FieldOffset(engineCl, "_cheatsEnabled");
			persistentOffset = Resolver.FieldOffset(engineCl, "_persistent");

			var heroBelt = Resolver.FindField(engineCl, "_heroBelt");
			heroBeltOffset = heroBelt.offset;
			heroBeltPointerOffset = Resolver.FieldOffset(heroBelt.type, "_pointer");

			var persistentCl = Resolver.FindClass("Hadesch::Persistent");
			genderOffset = Resolver.FieldOffset(persistentCl, "_gender");
			heroNameOffset = Resolver.FieldOffset(persistentCl, "_heroName");
			questOffset = Resolver.FieldOffset(persistentCl, "_quest");
			currentRoomIdOffset = Resolver.FieldOffset(persistentCl, "_currentRoomId");
			previousRoomIdOffset = Resolver.FieldOffset(persistentCl, "_previousRoomId");
			inventoryOffset = Resolver.FieldOffset(persistentCl, "_inventory");
			hintsAreEnabledOffset = Resolver.FieldOffset(persistentCl, "_hintsAreEnabled");

			LoadADSymbols(descriptorOffset, true);
		}

		public PersistentData ReadPersistent() {
			IntPtr persAddr = EngineAddr + persistentOffset;
			var pers = new PersistentData();
			pers.Gender = (Gender)MemoryReader.ReadByte(persAddr + genderOffset);
			pers.HeroName = ReadComString(persAddr + heroNameOffset);
			pers.Quest = (Quest)MemoryReader.ReadByte(persAddr + questOffset);
			pers.HintsEnabled = MemoryReader.ReadByte(persAddr + hintsAreEnabledOffset)!=0;
			pers.CurrentRoomId = (RoomId)MemoryReader.ReadByte(persAddr + currentRoomIdOffset);
			pers.PreviousRoomId = (RoomId)MemoryReader.ReadByte(persAddr + previousRoomIdOffset);

			return pers;
		}

		public bool CheatsEnabled {
			get {
				return MemoryReader.ReadByte(EngineAddr + cheatsEnabledOffset) != 0;
			}
		}

		public class PersistentData {
			public Gender Gender;
			public string HeroName;
			public Quest Quest;
			public bool HintsEnabled;
			public RoomId CurrentRoomId;
			public RoomId PreviousRoomId;
		}

		public enum Gender {
			Female = 0,
			Male = 1,
			Unknown = 2
		}

		public enum Quest {
			NoQuest,
			CreteQuest,
			TroyQuest,
			MedusaQuest,
			RescuePhilQuest,
			EndGame
		}

		public enum RoomId {
			InvalidRoom = 0,
			IntroRoom = 1,
			OlympusRoom = 2,
			WallOfFameRoom = 3,
			SeriphosRoom = 4,
			AthenaRoom = 5,
			MedIsleRoom = 6,
			MedusaPuzzle = 7,
			ArgoRoom = 8,
			TroyRoom = 9,
			CatacombsRoom = 10,
			PriamRoom = 11,
			TrojanHorsePuzzle = 12,
			CreteRoom = 13,
			MinosPalaceRoom = 14,
			DaedalusRoom = 15,
			MinotaurPuzzle = 16,
			VolcanoRoom = 17,
			RiverStyxRoom = 18,
			HadesThroneRoom = 19,
			FerrymanPuzzle = 20,
			MonsterPuzzle = 21,
			Quiz = 22,
			CreditsRoom = 23,
			OptionsRoom = 24,
		}
	}
}
