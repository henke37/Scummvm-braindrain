using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrainLib.Engines {
	public class HyperspaceDeliveryBoyEngineAccessor : BaseEngineAccessor {

		#region Symbol data
		//Main engine
		int gameStateOffset;
		int actionModeOffset;
		int currentMapnameOffset;
		const int currentMapnameSize = 64;
		int soundOffset;

		//Sound class
		int song1Offset;
		int song2Offset;

		//Song class
		int songPlayingOffset;
		int songSongOffset;
		#endregion

		private IntPtr soundAddr;

		public HyperspaceDeliveryBoyEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
			LoadSemiStaticData();
		}

		private void LoadSemiStaticData() {
			soundAddr = MemoryReader.ReadIntPtr(EngineAddr + soundOffset);
		}

		public override string GameId => "hbd";

		internal override void LoadSymbols() {
			var engineCl = Resolver.FindClass("HDB::HDBGame");
			gameStateOffset = Resolver.FieldOffset(engineCl, "_gameState");
			actionModeOffset = Resolver.FieldOffset(engineCl, "_actionMode");
			currentMapnameOffset = Resolver.FieldOffset(engineCl, "_currentMapname");
			soundOffset = Resolver.FieldOffset(engineCl, "_sound");

			var sndCl = Resolver.FindClass("HDB::Sound");
			song1Offset = Resolver.FieldOffset(sndCl, "_song1");
			song2Offset = Resolver.FieldOffset(sndCl, "_song2");

			var songCl = Resolver.FindClass("HDB::Song");
			songPlayingOffset = Resolver.FieldOffset(songCl, "_playing");
			songSongOffset = Resolver.FieldOffset(songCl, "_song");
		}

		public HDBState GetState() {
			var state = new HDBState();
			state.State = (GameState)MemoryReader.ReadInt32(EngineAddr + gameStateOffset);
			state.ActionMode = 1 == MemoryReader.ReadInt32(EngineAddr + actionModeOffset);
			state.CurrentMap = MemoryReader.ReadNullTermString(EngineAddr + currentMapnameOffset);
			return state;
		}

		public MusicState GetMusicState() {

			MusicState.Song GetSong(IntPtr songAddr) {
				var song = new MusicState.Song();
				song.Playing = MemoryReader.ReadByte(songAddr + songPlayingOffset) != 0;
				song.SongID = (SoundType)MemoryReader.ReadInt16(songAddr + songSongOffset);
				return song;
			}

			var state = new MusicState();
			state.Song1 = GetSong(soundAddr+song1Offset);
			state.Song2 = GetSong(soundAddr+song2Offset);
			return state;
		}

		public class HDBState {
			public string CurrentMap;
			public bool ActionMode;
			public GameState State;
		}

		public enum GameState {
			Title,
			Menu,
			Play,
			Loading
		}

		public class MusicState {
			public Song Song1;
			public Song Song2;
			public class Song {
				public bool Playing;
				public SoundType SongID;

				public override string ToString() {
					return SongID.ToString();
				}
			}

			public override string ToString() {
				if(!Song1.Playing && !Song2.Playing) return "SONG_NONE";
				if(Song1.Playing && !Song2.Playing) return Song1.ToString();
				if(!Song1.Playing && Song2.Playing) return Song2.ToString();
				return $"{Song1} {Song2}";
			}
		}

		public enum SoundType {
			SONG_NONE,
			SND_GUI_INPUT,
			SND_MAIL_PROCESS,
			SND_MONKEY_OOHOOH,
			SND_GET_GEM,
			SND_MENU_ACCEPT,
			SND_MENU_BACKOUT,
			SND_MENU_SLIDER,
			SND_DIALOG_CLOSE,
			SND_CRATE_SLIDE,
			SND_LIGHT_SLIDE,
			SND_HEAVY_SLIDE,
			SND_POP,
			SND_TELEPORT,
			SND_FOOTSTEPS,
			SND_SPLASH,
			SND_CELLHOLDER_USE_REJECT,
			SND_CHICKEN_AMBIENT,
			SND_FERRET_SQUEAK,
			SND_SWITCH_USE,
			SND_MOVE_SELECTION,
			SND_NOTICE,
			SND_MAINTBOT_WHOOSH1,
			SND_MAINTBOT_WHOOSH2,
			SND_SHIPMOVING_INTRO,
			SND_DIALOG_OPEN,
			SND_TOUCHPLATE_CLICK,
			SND_DOOR_OPEN_CLOSE,
			SND_MBOT_HYEAH,
			SND_MBOT_YEAH,
			SND_MBOT_WHISTLE1,
			SND_CLUB_MISS,
			SND_CLUB_HIT_METAL,
			SND_CLUB_HIT_FLESH,
			SND_FROG_LICK,
			SND_ROBOT_STUNNED,
			SND_QUEST_FAILED,
			SND_GET_MONKEYSTONE,
			SND_INSERT_CELL,
			SND_CABINET_OPEN,
			SND_CABINET_CLOSE,
			SND_MAILSORTER_HAPPY,
			SND_QUEST_COMPLETE,
			SND_TRY_AGAIN,
			SND_AIRLOCK_CLOSE,
			SND_BYE,
			SND_FART,
			SND_FART2,
			SND_GUY_UHUH,
			SND_GUY_DYING,
			SND_GEM_THROW,
			SND_INV_SELECT,
			SND_INFOCOMP,
			SND_CLOCK_BONK,
			SND_HDB,
			SND_VORTEX_SAVE,
			SND_GET_GOO,
			SND_MANNY_CRASH,
			SND_BARREL_EXPLODE,
			SND_BARREL_MELTING,
			SND_CHICKEN_BAGAWK,
			SND_CHICKEN_DEATH,
			SND_GET_THING,
			SND_STEPS_ICE,
			SND_FOURFIRE_TURN,
			SND_FOUR_FIRE,
			SND_SHOCKBOT_AMBIENT,
			SND_SHOCKBOT_SHOCK,
			SND_RAILRIDER_ONTRACK,
			SND_RAILRIDER_TASTE,
			SND_RAILRIDER_EXIT,
			SND_GUY_FRIED,
			SND_MAILSORTER_UNHAPPY,
			SND_GET_CLUB,
			SND_BUZZFLY_FLY,
			SND_BUZZFLY_STUNNED,
			SND_BUZZFLY_STING,
			SND_FATFROG_STUNNED,
			SND_NOPUSH_SIZZLE,
			SND_OMNIBOT_FIRE,
			SND_RIGHTBOT_TURN,
			SND_RIGHTBOT_STUNNED,
			SND_MONKEY_WIN,
			SND_FALL_DOWN_HOLE,
			SND_MBOT_HMMM,
			SND_MBOT_HMMM2,
			SND_MBOT_DEATH,
			SND_MBOT_WHISTLE2,
			SND_MBOT_WHISTLE3,
			SND_DEADEYE_AMB01,
			SND_DEADEYE_AMB02,
			SND_DEADEYE_ATTACK01,
			SND_DEADEYE_ATTACK02,
			SND_FROG_RIBBIT1,
			SND_FROG_RIBBIT2,
			SND_MEERKAT_BITE,
			SND_BRIDGE_EXTEND,
			SND_BRIDGE_START,
			SND_BRIDGE_END,
			SND_MACHINE_AMBIENT1,
			SND_GET_STUNNER,
			SND_GET_SLUG,
			SND_GUY_DROWN,
			SND_GUY_GRABBED,
			SND_PANIC,
			SND_PANIC_COUNT,
			SND_PANIC_DEATH,
			SND_LASER_LOOP,
			SND_SLOT_WIN,
			SND_SLOT_SPIN,
			SND_SLOT_STOP,
			SND_GOOD_FAERIE_AMBIENT,
			SND_GOOD_FAERIE_SPELL,
			SND_GOOD_FAERIE_STUNNED,
			SND_ICEPUFF_WARNING,
			SND_ICEPUFF_THROW,
			SND_ICEPUFF_STUNNED,
			SND_ICEPUFF_APPEAR,
			SND_GUY_PLUMMET,
			SND_PUSH_DIVERTER,
			SND_TURNBOT_TURN,
			SND_PUSHBOT_STRAIN,
			SND_MONKEYSTONE_SECRET_STAR,
			SND_OMNIBOT_AMBIENT,
			SND_PUSHBOT_STUNNED,
			SND_MEERKAT_WARNING,
			SND_MEERKAT_APPEAR,
			SND_MEERKAT_STUNNED,
			SND_TURNBOT_STUNNED,
			SND_DRAGON_WAKE,
			SND_DRAGON_FALLASLEEP,
			SND_DRAGON_BREATHEFIRE,
			SND_BADFAIRY_AMBIENT,
			SND_BADFAIRY_SPELL,
			SND_BADFAIRY_STUNNED,
			SND_DEMIGOD_AMBIENT,
			SND_DEMIGOD_HOLYSPEECH,
			SND_DEMIGOD_UNHAPPY,
			SND_GATEPUDDLE_AMBIENT,
			SND_GATEPUDDLE_DISSIPATE,
			SND_GATEPUDDLE_SPAWN,
			SND_REALSLOT_SPIN,
			SND_REALSLOT_STOP,
			SND_REALSLOT_WIN,
			SND_SLUG_FIRE,
			SND_SLUG_HIT,
			SND_STUNNER_FIRE,
			SND_UNLOCKED_ITEM,

			SONG_TITLE,
			SONG_MENU,
			SONG_ROBO,
			SONG_MEXI,
			SONG_BASSO,
			SONG_WIND,
			SONG_INDUSTRO,
			SONG_JACKIN,
			SONG_SNEAKERS,
			SONG_QUIET,
			SONG_JEEBIES,
			SONG_VIBRACIOUS,
			SONG_ROMANTIC,
			SONG_ARETHERE,

			SONG_CORRIDOR,
			SONG_MOKE,
			SONG_TILES,
			SONG_DARKVIB,
			SONG_EXPER
		}
	}
}
