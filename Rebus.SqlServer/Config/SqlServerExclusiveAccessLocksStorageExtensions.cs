using System;
using Rebus.ExclusiveLocks;
using Rebus.Logging;
using Rebus.Threading;
using Rebus.Time;

namespace Rebus.Config;

/// <summary>
/// Configuration extensions for exclusive locks storage
/// </summary>
public static class SqlServerExclusiveAccessLocksStorageExtensions
{
    /// <summary>
    /// Configure Rebus to use a SQL Server table for managing exclusive locks
    /// </summary>
    /// <param name="configurer">Reference to the configurer instance</param>
    /// <param name="options">Options for the transport</param>
    /// <param name="tableName">Name of the table to store the locks in</param>
    /// <returns>Instance of options for fluent configuration</returns>
    public static SqlServerExclusiveAccessLockOptions UseSqlServer(this StandardConfigurer<IExclusiveAccessLock> configurer, SqlServerExclusiveAccessLockOptions options, string tableName)
    {
        if (configurer == null) throw new ArgumentNullException(nameof(configurer));

        // Register the locker instance when we are configured
        configurer.OtherService<IExclusiveAccessLock>().Register(context =>
        {
            var connectionProvider = options.ConnectionProviderFactory(context);
            var rebusLoggerFactory = context.Get<IRebusLoggerFactory>();
            var asyncTaskFactory = context.Get<IAsyncTaskFactory>();
            var rebusTime = context.Get<IRebusTime>();

            var locker = new SqlServerExclusiveAccessLock(connectionProvider, tableName, rebusLoggerFactory, asyncTaskFactory, rebusTime, options);
            if (options.EnsureTablesAreCreated)
            {
                locker.EnsureTableIsCreated();
            }

            return locker;
        });

        return options;
    }
}