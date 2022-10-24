using System;
using Rebus.ExclusiveLocks;

namespace Rebus.Config;

/// <summary>
/// Provides extensions for managing <seealso cref="SqlServerExclusiveAccessLock"/>
/// </summary>
public static class SqlServerExclusiveAccessLockOptionsExtensions
{
    /// <summary>
    /// Opts the client out of any table creation
    /// </summary>
    public static TTransportOptions OptOutOfTableCreation<TTransportOptions>(this TTransportOptions options) where TTransportOptions : SqlServerExclusiveAccessLockOptions
    {
        options.EnsureTablesAreCreated = false;
        return options;
    }

    /// <summary>
    /// Sets if table creation is allowed
    /// </summary>
    public static TTransportOptions SetEnsureTablesAreCreated<TTransportOptions>(this TTransportOptions options, bool ensureTablesAreCreated) where TTransportOptions : SqlServerExclusiveAccessLockOptions
    {
        options.EnsureTablesAreCreated = ensureTablesAreCreated;
        return options;
    }

    /// <summary>
    /// Sets if table will be dropped automatically
    /// </summary>
    public static TTransportOptions SetAutoDeleteTable<TTransportOptions>(this TTransportOptions options, bool autoDeleteQueue) where TTransportOptions : SqlServerExclusiveAccessLockOptions
    {
        options.AutoDeleteTable = autoDeleteQueue;
        return options;
    }

    /// <summary>
    /// Sets the amount of time a lock will remain in the table before it is automatically cleared out
    /// </summary>
    public static TTransportOptions SetLockExpirationTimeout<TTransportOptions>(this TTransportOptions options, TimeSpan timeout) where TTransportOptions : SqlServerExclusiveAccessLockOptions
    {
        options.LockExpirationTimeout = timeout;
        return options;
    }

    /// <summary>
    /// Sets the delay between executions of the background cleanup task
    /// </summary>
    public static TTransportOptions SetExpiredLocksCleanupInterval<TTransportOptions>(this TTransportOptions options, TimeSpan interval) where TTransportOptions : SqlServerExclusiveAccessLockOptions
    {
        options.ExpiredLocksCleanupInterval = interval;
        return options;
    }
}
