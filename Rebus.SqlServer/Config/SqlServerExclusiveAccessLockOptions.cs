﻿using System;
using System.Threading.Tasks;
using Rebus.ExclusiveLocks;
using Rebus.Injection;
using Rebus.Logging;
using Rebus.SqlServer;

namespace Rebus.Config;

/// <summary>
/// Describes options used to configure the <seealso cref="SqlServerExclusiveAccessLock"/>
/// </summary>
public class SqlServerExclusiveAccessLockOptions : SqlServerOptions
{
    /// <summary>
    /// Create an instance of the transport with a pre-created <seealso cref="DbConnectionProvider"/>
    /// </summary>
    public SqlServerExclusiveAccessLockOptions(IDbConnectionProvider connectionProvider)
    {
        ConnectionProviderFactory = resolutionContext => connectionProvider;
    }

    /// <summary>
    /// Create an instance of the transport with a <paramref name="connectionProviderFactory"/> that can use the <see cref="IResolutionContext"/> to look up things
    /// </summary>
    public SqlServerExclusiveAccessLockOptions(Func<IResolutionContext, IDbConnectionProvider> connectionProviderFactory)
    {
        ConnectionProviderFactory = connectionProviderFactory ?? throw new ArgumentNullException(nameof(connectionProviderFactory));
    }

    /// <summary>
    /// Creates an instance of the transport connecting via <paramref name="connectionString"/>
    /// </summary>
    public SqlServerExclusiveAccessLockOptions(string connectionString, bool enlistInAmbientTransaction = false)
    {
        ConnectionProviderFactory = resolutionContext => new DbConnectionProvider(connectionString, resolutionContext.Get<IRebusLoggerFactory>(), enlistInAmbientTransaction);
    }

    /// <summary>
    /// Creates an instance of the transport with utilising an <seealso cref="IDbConnectionProvider"/> factory
    /// </summary>
    public SqlServerExclusiveAccessLockOptions(Func<Task<IDbConnection>> connectionFactory)
    {
        ConnectionProviderFactory = resolutionContext => new DbConnectionFactoryProvider(connectionFactory);
    }

    /// <summary>
    /// If true, the lock table will be automatically dropped on disposal
    /// </summary>
    internal bool AutoDeleteTable { get; set; }

    /// <summary>
    /// Gets the amount of time a lock will remain in the table before it is automatically cleared out
    /// </summary>
    internal TimeSpan? LockExpirationTimeout { get; set; }

    /// <summary>
    /// Gets the delay between executions of the background cleanup task
    /// </summary>
    internal TimeSpan? ExpiredLocksCleanupInterval { get; set; }
}
