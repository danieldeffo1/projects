import QtQuick 2.0

Image{
    id: root
    width: 100
    height: 47.63
    source: "pictures/car21.png"
    property string text
    transform: Rotation{
        angle: 90
    }
    Rectangle {
        id: _rect
        visible: mouse.containsMouse
        height: _text.height
        width: _text.width
        anchors.left: root.left
        anchors.top: root.top
        border.color: "black"
        border.width: 1
        transform: Rotation{
            angle: -90
        }
        Text {
            id: _text
            text: root.text
            font.pointSize: 16
            visible: mouse.containsMouse
        }
    }

    MouseArea{
        id: mouse
        hoverEnabled: true
        anchors.fill: parent
    }
}

