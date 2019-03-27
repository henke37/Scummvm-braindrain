using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace DebugHelp.RTTI {
	public class RTTIReader {
		private ProcessMemoryReader processMemoryReader;

		private Dictionary<uint, CompleteObjectLocator> completeObjectLocatorMap;
		private Dictionary<uint, TypeDescriptor> typeDescriptorMap;
		private Dictionary<uint, ClassHierarchyDescriptor> classDescriptorMap;

		public RTTIReader(ProcessMemoryReader processMemoryReader) {
			this.processMemoryReader = processMemoryReader ?? throw new ArgumentNullException(nameof(processMemoryReader));

			completeObjectLocatorMap = new Dictionary<uint, CompleteObjectLocator>();
			typeDescriptorMap = new Dictionary<uint, TypeDescriptor>();
			classDescriptorMap = new Dictionary<uint, ClassHierarchyDescriptor>();
		}

		private CompleteObjectLocator readObjPtr(uint objAddr) {
			var vtblPtrVal = processMemoryReader.ReadUInt32At(objAddr);
			var metaPtrVal = processMemoryReader.ReadUInt32At(vtblPtrVal - 4);

			if(completeObjectLocatorMap.TryGetValue(metaPtrVal, out CompleteObjectLocator objectLocator)) {
				return objectLocator;
			}

			var col = ReadCol(metaPtrVal);
			completeObjectLocatorMap[metaPtrVal] = col;

			return col;
		}

		private CompleteObjectLocator ReadCol(uint metaPtrVal) {
			CompleteObjectLocator.MemoryStruct memoryStruct = new CompleteObjectLocator.MemoryStruct();
			processMemoryReader.ReadStruct(metaPtrVal, ref memoryStruct);

			if(memoryStruct.Signature != 0) throw new InvalidDataException("Invalid COL signature");

			Debug.Assert(memoryStruct.pTypeDescriptor != 0, "pTypeDescriptor shouldn't be 0!");
			Debug.Assert(memoryStruct.pClassDescriptor != 0, "pClassDescriptor shouldn't be 0!");

			CompleteObjectLocator locator = new CompleteObjectLocator();
			locator.ConstructorDescriptorOffset = memoryStruct.ConstructorDescriptorOffset;
			locator.ObjectRootOffset = memoryStruct.ObjectRootOffset;
			locator.TypeDescriptor = GetTypeDescriptor(memoryStruct.pTypeDescriptor);
			locator.ClassHierarchyDescriptor = GetClassHierarchyDescriptor(memoryStruct.pClassDescriptor);

			return locator;
		}

		private ClassHierarchyDescriptor GetClassHierarchyDescriptor(uint pClassDescriptor) {
			ClassHierarchyDescriptor desc;
			if(classDescriptorMap.TryGetValue(pClassDescriptor, out desc)) return desc;
			desc = ReadClassHierarchyDescriptor(pClassDescriptor);
			classDescriptorMap[pClassDescriptor] = desc;
			return desc;
		}

		private ClassHierarchyDescriptor ReadClassHierarchyDescriptor(uint pClassDescriptor) {
			ClassHierarchyDescriptor.MemoryStruct memoryStruct = new ClassHierarchyDescriptor.MemoryStruct();
			processMemoryReader.ReadStruct(pClassDescriptor, ref memoryStruct);

			if(memoryStruct.Signature != 0) throw new InvalidDataException("Invalid class hierarchy signature");

			ClassHierarchyDescriptor desc = new ClassHierarchyDescriptor();
			desc.Flags = memoryStruct.Flags;
			desc.BaseClasses = new List<ClassHierarchyDescriptor.BaseClassDescriptor>((int)memoryStruct.numBaseClasses);

			var baseDescriptorPointers=processMemoryReader.ReadUInt32Array(memoryStruct.pBaseClassArray, memoryStruct.numBaseClasses);

			for(uint baseClassIndex = 0; baseClassIndex < memoryStruct.numBaseClasses; ++baseClassIndex) {
				desc.BaseClasses.Add(ReadBaseClassDescriptor(baseDescriptorPointers[baseClassIndex]));
			};

			return desc;
		}

		private ClassHierarchyDescriptor.BaseClassDescriptor ReadBaseClassDescriptor(uint baseClassDescriptorPointer) {
			ClassHierarchyDescriptor.BaseClassDescriptor.MemoryStruct memoryStruct = new ClassHierarchyDescriptor.BaseClassDescriptor.MemoryStruct();
			processMemoryReader.ReadStruct(baseClassDescriptorPointer, ref memoryStruct);

			ClassHierarchyDescriptor.BaseClassDescriptor desc = new ClassHierarchyDescriptor.BaseClassDescriptor();
			desc.TypeDescriptor = GetTypeDescriptor(memoryStruct.pTypeDescriptor);
			desc.DisplacementData = memoryStruct.DisplacementData;
			desc.NumContainedBases = memoryStruct.NumContainedBases;
			return desc;
		}

		private TypeDescriptor GetTypeDescriptor(uint pTypeDescriptor) {
			TypeDescriptor desc;
			if(typeDescriptorMap.TryGetValue(pTypeDescriptor, out desc)) return desc;
			desc = ReadTypeDescriptor(pTypeDescriptor);
			typeDescriptorMap[pTypeDescriptor] = desc;
			return desc;
		}

		private TypeDescriptor ReadTypeDescriptor(uint pTypeDescriptor) {
			return new TypeDescriptor(
				processMemoryReader.ReadNullTermString(pTypeDescriptor + 2 * 4)
			);
		}

		public string GetMangledClassNameFromObjPtr(uint objAddr) {
			var col = readObjPtr(objAddr);
			return col.TypeDescriptor.MangledName;
		}

		public bool IsObjectInheritingMangledType(uint objAddr, string mangledName) {
			readObjPtr(objAddr);
			throw new NotImplementedException();
		}
	}
}
