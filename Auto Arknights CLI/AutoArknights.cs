﻿using System;
using Microsoft.Extensions.Configuration;
using REVUnit.AutoArknights.Core;
using REVUnit.Crlib.Extension;
using REVUnit.Crlib.Input;

namespace REVUnit.AutoArknights.CLI
{
    public class AutoArknights : IDisposable
    {
        private readonly Automation _automation;

        public AutoArknights()
        {
            if (!Library.CheckIfSupported())
            {
                Log.Error("你的CPU不支持当前OpenCV构建，无法正常运行本程序，抱歉。");
                Console.ReadKey(true);
                return;
            }

            IConfiguration config = new ConfigurationBuilder().AddJsonFile(".\\Config.json").Build();
            _automation = new Automation(config["Adb:Path"], config["Adb:Remote"]);
        }

        public void Dispose()
        {
            _automation.Dispose();
        }

        public void Run()
        {
            var cin = new Cin();
            RepeatLevelJob.Mode mode = cin.Get<RepeatLevelJob.Mode>("输入模式");
            int repeatTime = -1;
            if (mode == RepeatLevelJob.Mode.SpecifiedTimes || mode == RepeatLevelJob.Mode.SpecTimesWithWait)
                repeatTime = cin.Get<int>("输入刷关次数");

            _automation.Schedule.Add(new RepeatLevelJob(_automation.Ui, mode, repeatTime));
            _automation.Schedule.ExecuteAll();

            Console.Beep();
            XConsole.AnyKey("所有任务完成");
        }
    }
}