using System;

namespace DebugHelp {
	[Flags]
	enum NameSearchOptions {
		None,
		CaseSensitive = 0x1,
		CaseInsensitive = 0x2,
		NameExt = 0x4,
		RegularExpression = 0x8,
		UndecoratedName = 0x10
	}
}