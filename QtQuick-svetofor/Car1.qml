import QtQuick 2.0

Image {
    id: root
    width: 100
    height: 47.63
    source: "pictures/car1.png"
    property string text
    Rectangle{
        visible: mouse.containsMouse
        height: _text.height
        width: _text.width
        anchors.left: root.right
        anchors.bottom: root.top
        border.color: "black"
        border.width: 1
        Text {
            id: _text
            text: root.text
            font.pointSize: 16
        }
    }
    MouseArea {
        id: mouse
        hoverEnabled: true
        anchors.fill: parent
    }
}
