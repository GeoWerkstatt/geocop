﻿namespace GeoCop.Api
{
    /// <summary>
    /// Supported log types available for download.
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// Log containing error messages and warnings.
        /// </summary>
        Log,

        /// <summary>
        /// Log containing error messages and warnings. Additionally the log
        /// follows the 'IliVErrors' model which can be used to visualize errors.
        /// </summary>
        Xtf,
    }
}
