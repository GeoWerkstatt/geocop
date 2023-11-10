global using Microsoft.VisualStudio.TestTools.UnitTesting;
using BenchmarkDotNet.Running;
using MyProject.Tests;

BenchmarkRunner.Run<DeliveryControllerTests>();
