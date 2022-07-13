﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MidiBard.MidiBard;

namespace MidiBard.Control.CharacterControl;

class PerformanceEvents
{
    private PerformanceEvents()
    {

    }

    public static PerformanceEvents Instance { get; } = new PerformanceEvents();

    private void EnteringPerformance()
    {
        if (Configuration.config.AutoOpenPlayerWhenPerforming)
            if (!SwitchInstrument.SwitchingInstrument)
                Ui.Open();

        if (Configuration.config.AutoSetBackgroundFrameLimit)
        {
            AgentConfigSystem.BackgroundFrameLimit = false;
            AgentConfigSystem.ApplyGraphicSettings();
        }
    }

    private void ExitingPerformance()
    {
        if (Configuration.config.AutoOpenPlayerWhenPerforming)
            if (!SwitchInstrument.SwitchingInstrument)
                Ui.Close();

        if (Configuration.config.AutoSetBackgroundFrameLimit)
        {
            AgentConfigSystem.BackgroundFrameLimit = true;
            AgentConfigSystem.ApplyGraphicSettings();
        }
    }

    private bool inPerformanceMode;

    public bool InPerformanceMode
    {
        set
        {
            if (value && !inPerformanceMode)
            {
                EnteringPerformance();
            }

            if (!value && inPerformanceMode)
            {
                ExitingPerformance();
            }

            inPerformanceMode = value;
        }
    }
}