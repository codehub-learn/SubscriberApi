using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace TestProjectLoans.Fixtures;

public class DatabaseFixture : IDisposable
{
    public DbConnection Connection { get; }

    public DatabaseFixture()
    {
        Connection = new SqlConnection("...");
        Connection.Open();
    }

    public void Dispose()
    {
        Connection.Dispose();
    }
}