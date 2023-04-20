#include "carcontrol.h"
#include <QDebug>

CarControl::CarControl():
    m_roadW{593},
    m_roadH{640},
    m_car1Size{100},
    m_car2Size{100},
    m_car1Speed{5.},
    m_car2Speed{2.5},
    m_car1Turns{0},
    m_car2Turns{0},
    m_car1Reversed{false},
    m_car2Reversed{false},
    m_stopLines{180, 180, 440, 420}

{
    m_car1.setX(10);
    m_car2.setY(10);
    m_car1.setY(310);
    m_car2.setX(310);

    timer1 = new QTimer(this);
    connect(timer1, &QTimer::timeout, this, &CarControl::car1Move);
    timer1->start(30);

    timer2 = new QTimer(this);
    connect(timer2, &QTimer::timeout, this, &CarControl::car2Move);
    timer2->start(30);
}

void CarControl::car1Move()
{
    if (!m_car1Reversed){
        if (!(m_car1.x() + m_car1Size >= m_stopLines[0] &&
                m_car1.x() + m_car1Size <= m_stopLines[0] + m_car1Speed) ||
                m_lights.lights()[0]==LightControl::green){
            m_car1.setX(m_car1.x() + m_car1Speed);
            if (m_car1.x() >= m_roadW){
                m_car1Reversed=true;
                m_car1.setY(m_car1.y() - 80);
                m_car1Turns++;
            }
            car1Changed();
        }
    }
    else {
        if (!(m_car1.x() <= m_stopLines[2] &&
                m_car1.x() >= m_stopLines[2] - m_car1Speed) ||
                m_lights.lights()[2]==LightControl::green){
            m_car1.setX(m_car1.x() - m_car1Speed);
            if (m_car1.x()+m_car1Size <= 0){
                m_car1Reversed=false;
                m_car1.setY(m_car1.y() + 80);
                m_car1Turns++;
            }
            car1Changed();
        }
    }

}

void CarControl::car2Move()
{
    if (!m_car2Reversed){
        if (!(m_car2.y() + m_car2Size >= m_stopLines[1] &&
                m_car2.y() + m_car2Size <= m_stopLines[1] + m_car2Speed) ||
                m_lights.lights()[1]==LightControl::green){
            m_car2.setY(m_car2.y() + m_car2Speed);
            if (m_car2.y() >= m_roadH){
                m_car2Reversed=true;
                m_car2.setX(m_car2.x() + 80);
                m_car2Turns++;
            }
            car2Changed();
        }
    }
    else {
        if (!(m_car2.y() <= m_stopLines[3] &&
                m_car2.y() >= m_stopLines[3] - m_car2Speed) ||
                m_lights.lights()[3]==LightControl::green){
            m_car2.setY(m_car2.y() - m_car2Speed);
            if (m_car2.y()+m_car2Size <= 0){
                m_car2Reversed=false;
                m_car2.setX(m_car2.x() - 80);
                m_car2Turns++;
            }
            car2Changed();
        }
    }
}

QPointF CarControl::car1() const
{
    return m_car1;
}

QPointF CarControl::car2() const
{
    return m_car2;
}

LightControl* CarControl::lights()
{
    return &m_lights;
}

bool CarControl::car1Reversed() const
{
    return m_car1Reversed;
}

bool CarControl::car2Reversed() const
{
    return m_car2Reversed;
}

QString CarControl::car1Turns() const
{
    return QString("Turns: ")+QString::number(m_car1Turns);
}

QString CarControl::car2Turns() const
{
    return QString("Turns: ")+QString::number(m_car2Turns);
}

void CarControl::restart()
{
    timer1->stop();
    timer2->stop();
    m_lights.restart();
    m_car1Turns=0;
    m_car2Turns=0;
    m_car1Reversed=false;
    m_car2Reversed=false;
    m_car1.setX(10);
    m_car2.setY(10);
    m_car1.setY(310);
    m_car2.setX(310);

    m_car1Speed=5.;
    m_car2Speed=2.5;
    timer1->start(30);
    timer2->start(30);

}

void CarControl::keyCatch(QString text)
{
    if(text=="c" || text=="C")
        restart();
    else if(text=="\033")
        QCoreApplication::exit();
}

