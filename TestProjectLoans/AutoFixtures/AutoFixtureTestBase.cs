using System;
using System.Collections.Generic;
using System.Text;

namespace TestProjectLoans.AutoFixtures;

using AutoFixture;
using AutoFixture.AutoMoq;

public abstract class AutoFixtureTestBase
{
    protected IFixture Fixture { get; }

    protected AutoFixtureTestBase()
    {
        Fixture = new Fixture()
            .Customize(new AutoMoqCustomization
            {
                ConfigureMembers = true
            });
    }
}