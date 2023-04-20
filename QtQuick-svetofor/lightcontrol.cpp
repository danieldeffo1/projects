#include "lightcontrol.h"

LightControl::LightControl():
    m_newLights{0,0,0,0},
    m_lights{green, red, green, red}
{
    timer=new QTimer(this);
    connect(timer, &QTimer::timeout, this, &LightControl::changeLightsYellow);
    timer->start(10000);

    colorTimer= new QTimer(this);
    connect(colorTimer, &QTimer::timeout, this, &LightControl::changeLightsOnColor);
}

QList<int> LightControl::lights()
{
    return m_lights;
}

void LightControl::changeLightsYellow()
{
    for(int i=0;i<m_newLights.length();i++)
        m_newLights[i]=m_lights[i]==green ? red : green;

    m_lights={yellow, yellow, yellow, yellow};
    colorTimer->start(2000);
    lightsChanged();
}

void LightControl::changeLightsOnColor()
{
    colorTimer->stop();
    m_lights=m_newLights;
    lightsChanged();
}

void LightControl::restart()
{
    timer->stop();
    colorTimer->stop();
    m_newLights={0,0,0,0};
    m_lights={green, red, green, red};
    timer->start(10000);
    lightsChanged();
}
