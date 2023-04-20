#ifndef LIGHTCONTROL_H
#define LIGHTCONTROL_H
#include <QObject>
#include <QList>
#include <QTimer>

class LightControl: public QObject
{
    Q_OBJECT
    Q_PROPERTY(QList<int> lights READ lights NOTIFY lightsChanged)
public:
    LightControl();
    QList<int> lights();
    void changeLightsYellow();
    void changeLightsOnColor();
    void restart();
    enum lights{
        green,
        yellow,
        red
    };
private:
    QList<int> m_newLights;
    QList<int> m_lights;
    QTimer *timer;
    QTimer *colorTimer;
signals:
    void lightsChanged();
};

#endif // LIGHTCONTROL_H
