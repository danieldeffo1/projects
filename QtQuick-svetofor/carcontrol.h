#ifndef CARCONTROL_H
#define CARCONTROL_H
#include <QObject>
#include <QTimer>
#include <QCoreApplication>
#include <QPointF>
#include <QString>
#include "lightcontrol.h"


class CarControl: public QObject
{
    Q_OBJECT
    Q_PROPERTY(QPointF car1 READ car1 NOTIFY car1Changed)
    Q_PROPERTY(QPointF car2 READ car2 NOTIFY car2Changed)
    Q_PROPERTY(bool car1Reversed READ car1Reversed NOTIFY car1Changed)
    Q_PROPERTY(bool car2Reversed READ car2Reversed NOTIFY car2Changed)
    Q_PROPERTY(QString car1Turns READ car1Turns NOTIFY car1Changed)
    Q_PROPERTY(QString car2Turns READ car2Turns NOTIFY car2Changed)
    Q_PROPERTY(LightControl* lights READ lights)
public:
    CarControl();
    void car1Move();
    void car2Move();
    QPointF car1() const;
    QPointF car2() const;

    LightControl* lights();

    bool car1Reversed() const;

    bool car2Reversed() const;

    QString car1Turns() const;
    QString car2Turns() const;

    Q_INVOKABLE void restart();

    Q_INVOKABLE void keyCatch(QString text);

private:
    int m_roadW;
    int m_roadH;
    int m_car1Size;
    int m_car2Size;
    float m_car1Speed;
    float m_car2Speed;
    int m_car1Turns;
    int m_car2Turns;
    QPointF m_car1;
    QPointF m_car2;
    bool m_car1Reversed;
    bool m_car2Reversed;
    int m_stopLines[4];
    LightControl m_lights;
    QTimer *timer1;
    QTimer *timer2;
signals:
    void car1Changed();
    void car2Changed();
};

#endif // CARCONTROL_H
