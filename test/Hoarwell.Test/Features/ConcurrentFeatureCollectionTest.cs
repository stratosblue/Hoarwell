// 代码由 AI 自动生成

using System.Collections.Concurrent;

namespace Hoarwell.Features.Tests;

[TestClass]
public class ConcurrentFeatureCollectionTest
{
    #region Public 方法

    [TestMethod]
    public void Should_EnumerateProperties_Correctly()
    {
        var feature = new ConcurrentFeatureCollection();
        var expected = new TestTypeA();
        feature.Set(expected);
        feature.Set("text");

        var properties = new List<KeyValuePair<Type, object?>>();
        foreach (var pair in feature)
        {
            properties.Add(pair);
        }

        Assert.HasCount(2, properties);
        Assert.IsTrue(properties.Any(p => p.Key == typeof(TestTypeA) && ReferenceEquals(p.Value, expected)));
        Assert.IsTrue(properties.Any(p => p.Key == typeof(string) && (string)p.Value! == "text"));
    }

    [TestMethod]
    public void Should_GetGenericProperty_Correctly()
    {
        var feature = new ConcurrentFeatureCollection();
        var expected = "test value";

        feature.Set(expected);
        var result = feature.Get<string>();

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void Should_InitializeDictionaryOnlyOnce()
    {
        var feature = new ConcurrentFeatureCollection();

        feature.Set(new TestTypeB());
        var initialDictionary = GetInternalDictionary(feature);
        feature.Set(new TestTypeA());

        Assert.AreSame(initialDictionary, GetInternalDictionary(feature));
    }

    [TestMethod]
    public void Should_NotThrow_When_SettingNullValue()
    {
        var feature = new ConcurrentFeatureCollection();

        feature.Set<string>(null);
        var result = feature.Get<string>();

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task Should_Parallel_Process_Correctly()
    {
        using var manualResetEvent = new ManualResetEvent(false);

        for (int i = 0; i < 100; i++)
        {
            manualResetEvent.Reset();

            var feature = new ConcurrentFeatureCollection();

            var tasks = Enumerable.Range(0, 500)
                                  .Select(_ => Task.Run(() =>
                                  {
                                      manualResetEvent.WaitOne();
                                      var expected = new TestTypeA();
                                      feature.Set(expected);
                                  }, TestContext.CancellationToken)).ToList();

            manualResetEvent.Set();

            await Task.WhenAll(tasks);

            Assert.IsNotNull(feature.Get<TestTypeA>());
        }
    }

    [TestMethod]
    public void Should_RemoveProperty_Correctly()
    {
        var feature = new ConcurrentFeatureCollection();
        feature.Set("value");

        bool removed = feature.Remove<string>();
        var result = feature.Get<string>();

        Assert.IsTrue(removed);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Should_ReturnDefault_When_PropertyNotExist()
    {
        var feature = new ConcurrentFeatureCollection();
        var result = feature.Get<string>();

        Assert.IsNull(result);
    }

    [TestMethod]
    public void Should_ReturnEmptyEnumerator_When_NoProperties()
    {
        var feature = new ConcurrentFeatureCollection();

        int count = 0;
        foreach (var _ in feature)
        {
            count++;
        }

        Assert.AreEqual(0, count);
    }

    [TestMethod]
    public void Should_StoreAndRetrieveProperty_Correctly()
    {
        var feature = new ConcurrentFeatureCollection();
        var testObj = new object();

        feature.Set<object>(testObj);
        var result = feature.Get<object>();

        Assert.AreSame(testObj, result);
    }

    [TestMethod]
    public void Should_TryGetReturnFalse_When_PropertyNotExist()
    {
        var feature = new ConcurrentFeatureCollection();

        bool found = feature.TryGet<string>(out var result);

        Assert.IsFalse(found);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Should_TryGetReturnTrue_When_PropertyExists()
    {
        var feature = new ConcurrentFeatureCollection();
        var expected = new TestTypeA();
        feature.Set(expected);

        bool found = feature.TryGet<TestTypeA>(out var result);

        Assert.IsTrue(found);
        Assert.AreEqual(expected, result);
    }

    #endregion Public 方法

    #region Private 方法

    public TestContext TestContext { get; set; }

    // 提取重复操作为私有方法
    private static ConcurrentDictionary<Type, object?>? GetInternalDictionary(ConcurrentFeatureCollection feature)
    {
        var field = typeof(ConcurrentFeatureCollection).GetField("_features",
                     System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return field?.GetValue(feature) as ConcurrentDictionary<Type, object?>;
    }

    #endregion Private 方法
}

// 测试使用的辅助类型
internal class TestTypeA
{ }

internal class TestTypeB
{ }
