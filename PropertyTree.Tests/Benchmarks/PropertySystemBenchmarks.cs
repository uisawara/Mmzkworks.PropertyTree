using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using works.mmzk.PropertyTree;

namespace PropertyTree.Tests.Benchmarks
{
    [MemoryDiagnoser]
    [SimpleJob]
    [JsonExporter]
    public class PropertySystemBenchmarks
    {
        private PropertyGroup _rootGroup;
        private PropertyGroup _subGroup;
        private StringProperty _stringProperty;
        private IntProperty _intProperty;
        private FloatProperty _floatProperty;
        private BoolProperty _boolProperty;
        private EnumProperty<TestEnum> _enumProperty;

        [GlobalSetup]
        public void Setup()
        {
            _rootGroup = new PropertyGroup("RootGroup");
            _subGroup = new PropertyGroup("SubGroup");
            _stringProperty = new StringProperty("TestString");
            _intProperty = new IntProperty("TestInt");
            _floatProperty = new FloatProperty("TestFloat");
            _boolProperty = new BoolProperty("TestBool");
            _enumProperty = new EnumProperty<TestEnum>("TestEnum");

            _rootGroup.Add(_subGroup);
            _subGroup.Add(_stringProperty);
            _subGroup.Add(_intProperty);
            _subGroup.Add(_floatProperty);
            _subGroup.Add(_boolProperty);
            _subGroup.Add(_enumProperty);
        }

        [Benchmark]
        public void PropertyCreation_Simple()
        {
            var property = new StringProperty("BenchmarkProperty");
        }

        [Benchmark]
        public void PropertyCreation_WithAdapter()
        {
            var property = new StringProperty("BenchmarkProperty");
            var adapter = new StringPropertyAdapter(property);
        }

        [Benchmark]
        public void PropertyGroup_AddProperty()
        {
            var group = new PropertyGroup("BenchmarkGroup");
            var property = new StringProperty("BenchmarkProperty");
            group.Add(property);
        }

        [Benchmark]
        public void PropertyGroup_RemoveProperty()
        {
            var group = new PropertyGroup("BenchmarkGroup");
            var property = new StringProperty("BenchmarkProperty");
            group.Add(property);
            group.Remove(property);
        }

        [Benchmark]
        public void PropertyValue_GetSet()
        {
            _stringProperty.Value = "TestValue";
            var value = _stringProperty.Value;
        }

        [Benchmark]
        public void PropertyValue_IntGetSet()
        {
            _intProperty.Value = 42;
            var value = _intProperty.Value;
        }

        [Benchmark]
        public void PropertyValue_FloatGetSet()
        {
            _floatProperty.Value = 3.14f;
            var value = _floatProperty.Value;
        }

        [Benchmark]
        public void PropertyValue_BoolGetSet()
        {
            _boolProperty.Value = true;
            var value = _boolProperty.Value;
        }

        [Benchmark]
        public void PropertyValue_EnumGetSet()
        {
            _enumProperty.Value = TestEnum.Value2;
            var value = _enumProperty.Value;
        }

        [Benchmark]
        public void PropertyExtensions_GetFullPath()
        {
            var path = _stringProperty.GetFullPath();
        }

        [Benchmark]
        public void PropertyExtensions_GetDepth()
        {
            var depth = _stringProperty.GetDepth();
        }

        [Benchmark]
        public void PropertyExtensions_GetParentChain()
        {
            var parentChain = _stringProperty.GetParentChain();
        }

        [Benchmark]
        public void PropertyExtensions_FindByPath()
        {
            var result = _rootGroup.FindByPath("SubGroup.TestString");
        }

        [Benchmark]
        public void PropertyExtensions_FindByPattern()
        {
            var results = _rootGroup.FindByPattern("SubGroup.*");
        }

        [Benchmark]
        public void PropertyExtensions_GetAllProperties()
        {
            var allProperties = _rootGroup.GetAllProperties();
        }

        [Benchmark]
        public void PropertyAdapter_StringAdapter()
        {
            var adapter = new StringPropertyAdapter(_stringProperty);
            adapter.Value = "AdapterValue";
            var value = adapter.Value;
        }

        [Benchmark]
        public void PropertyAdapter_IntAdapter()
        {
            var adapter = new IntPropertyAdapter(_intProperty);
            adapter.Value = 100;
            var value = adapter.Value;
        }

        [Benchmark]
        public void PropertyAdapter_FloatAdapter()
        {
            var adapter = new FloatPropertyAdapter(_floatProperty);
            adapter.Value = 2.718f;
            var value = adapter.Value;
        }

        [Benchmark]
        public void PropertyAdapter_BoolAdapter()
        {
            var adapter = new BoolPropertyAdapter(_boolProperty);
            adapter.Value = false;
            var value = adapter.Value;
        }

        [Benchmark]
        public void ComplexOperation_PropertyHierarchy()
        {
            var root = new PropertyGroup("Root");
            var sub1 = new PropertyGroup("Sub1");
            var sub2 = new PropertyGroup("Sub2");
            var prop1 = new StringProperty("Prop1");
            var prop2 = new IntProperty("Prop2");

            root.Add(sub1);
            sub1.Add(sub2);
            sub2.Add(prop1);
            sub2.Add(prop2);

            prop1.Value = "Value1";
            prop2.Value = 123;

            var path = prop1.GetFullPath();
            var depth = prop1.GetDepth();
            var parentChain = prop1.GetParentChain();
        }

        public enum TestEnum
        {
            Value1,
            Value2,
            Value3
        }
    }
} 