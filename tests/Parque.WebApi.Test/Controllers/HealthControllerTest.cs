using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Parque.WebApi.Controllers;

namespace Parque.WebApi.Test.Controllers;

[TestClass]
public class HealthControllerTest
{
    private HealthController? _controller;

    [TestInitialize]
    public void Initialize()
    {
        _controller = new HealthController();
    }

    [TestMethod]
    public void Get_ShouldReturnHealthStatus()
    {
        var result = _controller!.Get();

        Assert.IsNotNull(result);
        var resultType = result.GetType();
        var vProperty = resultType.GetProperty("v");
        var aliveProperty = resultType.GetProperty("alive");

        Assert.IsNotNull(vProperty);
        Assert.IsNotNull(aliveProperty);
        Assert.AreEqual("1.0", vProperty.GetValue(result));
        Assert.AreEqual(true, aliveProperty.GetValue(result));
    }
}
