﻿using Furion.DependencyInjection;
using System;

namespace DataCollect.Application.UserCenter
{
    /// <summary>
    /// 登录模型
    /// </summary>
    [SkipScan]
    public class LoginOutput
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// 头像（OSS地址）
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        /// 最近登录时间
        /// </summary>
        public DateTimeOffset SigninedTime { get; set; }
    }
}