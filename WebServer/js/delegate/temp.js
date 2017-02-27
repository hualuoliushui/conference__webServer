//定义绘图类型：标签矩形LabeledRect
var LabeledRect = fabric.util.createClass(fabric.Rect, {

    type: 'labeledRect',

    initialize: function (options) {
        options || (options = {});

        this.callSuper('initialize', options);
        this.set('label', options.label || '');
    },

    toObject: function () {
        return fabric.util.object.extend(this.callSuper('toObject'), {
            label: this.get('label')
        });
    },

    _render: function (ctx) {
        this.callSuper('_render', ctx);

        ctx.font = '20px Helvetica';
        ctx.fillStyle = '#333';
        ctx.fillText(this.label, -this.width / 2, -this.height / 2 + 20);
    }
});

//###################################################################
//定义全局变量
//数据数组
var datas = new Array();
//画布
var canvas = new fabric.Canvas('seatArrange', {
    hoverCursor: 'pointer',
    selection: false, // disable group selection
    allowTouchScrolling: true
});
var _canvas = document.getElementById("seatArrange");

//画布尺寸
var width = canvas.get('width');
var height = canvas.get('height');

//会议名称
var meetingName = "会议桌";

//声明存储座位的数组
var seats;
//定义座位
function seatClass(id, name, isSet, direction, x, y) {
    this.isSet = isSet || 1;
    this.id = id;
    this.name = name;
    this.direction = direction || 0;
    this.x = x || 0;
    this.y = y || 0;
    this.rect = "";

    this.reset = function () {
        this.isSet = 0;
        this.rect = undefined;
    }
}

//声明座位
var seat;
//定义座位在画布中的尺寸
var seatWidth;
var seatHeight;
//根据画布大小、座位大小，定义座位在画布中的行数、列数（包括桌子）
var seatInCanvas_NumColumns;
var seatInCanvas_NumRows;

//根据桌子尺寸、画布大小，定义桌子 在以座位尺寸为基础的范围(坐标以左上角为原点进行计算）
var tableLeft_ColumnIndex;
var tableTop_RowIndex;

var tableWidth_NumColumns;
var tableHeight_NumRows;

var tableRight_ColumnIndex;
var tableBottom_RowIndex;

//定义桌子参数//以像素为基本单位
var table;
var tableLeft;
var tableTop;
var tableWidth;
var tableHeight;

//###################################################################
//定义函数

//根据坐标返回线段
function make_line(coordnates) {
    return new fabric.Line(coordnates,
        {
            fill: 'red',
            stroke: 'black',
            strokeWidth: 1,
            hasControlable: false,
            selectable: false //不可选
        });
}

//返回文本对象
function make_LabeledRect(_left, _top, _width, _height, _text, _fill, _selectable) {
    console.log(_selectable);

    _fill = _fill || '#faa';
    return new LabeledRect({
        left: _left,
        top: _top,
        width: _width,
        height: _height,
        label: _text,
        selectable: _selectable,// 是否可选
        hasControls: false, //不允许修改形状
        hasBorders: false,// 取消选择时，显示边界
        fill: _fill
    });
}

//在画布中绘制以座位尺寸为基础的网格线
function draw_grid() {
    var index = 1;
    //绘制横线
    for (index = 1; index < seatInCanvas_NumRows; index++) {
        var x0 = tableLeft_ColumnIndex * seatWidth;
        var y0 = index * seatHeight;
        var x1 = tableRight_ColumnIndex * seatWidth;
        var y1 = y0;

        if (index >= tableTop_RowIndex && index <= tableBottom_RowIndex) {
            var x0 = 0;
            var x1 = width;
        }

        canvas.add(make_line([x0, y0, x1, y1]));
    }
    //绘制竖线
    for (index = 1; index < seatInCanvas_NumColumns; index++) {
        var x0 = index * seatWidth;
        var y0 = tableTop_RowIndex * seatHeight;
        var x1 = x0;
        var y1 = tableBottom_RowIndex * seatHeight;
        if (index >= tableLeft_ColumnIndex && index <= tableRight_ColumnIndex) {
            var y0 = 0;
            var y1 = height;
        }
        canvas.add(make_line([x0, y0, x1, y1]));
    }
}

//绘制桌子
function draw_tableBorder() {
    canvas.add(make_LabeledRect(tableLeft, tableTop, tableWidth, tableHeight, meetingName, 'green', false));
}

//从画布坐标 映射到 方位坐标系
function position2DirctionXY(position) {
    var direction = 0;
    var x = 0;
    var y = 0;

    return { direction: direction, x: x, y: y };
}

//画布中，座位图形坐标修正// 避免重叠，符合网格
function position_change(left, top) { // x,y

    //以座位尺寸为基本单位
    left = Math.floor(left / seatWidth);
    top = Math.floor(top / seatHeight);

    //console.log("left:" + left + " top:" + top);
    //console.log("tableTop_Row:" + tableTop_Row);
    //console.log("tableBottom_Row:" + tableBottom_Row);
    //console.log("tableLeft_Column:" + tableLeft_Column);
    //console.log("tableRight_Column:" + tableRight_Column);

    //避免与桌子边框坐标重叠
    if (top >= tableTop_RowIndex
        && left >= tableLeft_ColumnIndex
        && top < tableBottom_RowIndex
        && left < tableRight_ColumnIndex) { //位于桌子中间
        console.log("与桌子重叠");
        //坐标修正 以桌子的一半为分割线 修正竖直方向
        if (top > (tableTop_RowIndex + tableBottom_RowIndex) / 2) {
            //修正
            top = tableBottom_RowIndex;
        } else {
            top = tableTop_RowIndex - 1;
        }
        //避免 出 座位网格边缘
    } else {
        if (top < tableTop_RowIndex && (left < tableLeft_ColumnIndex
            || left >= tableRight_ColumnIndex)) {
            console.log("位于左上方或右上方");
            top = tableTop_RowIndex;
        } else if (top >= tableBottom_RowIndex && (left < tableLeft_ColumnIndex
            || left >= tableRight_ColumnIndex)) {
            console.log("/位于左下方或右下方");
            top = tableBottom_RowIndex - 1;
        }
    }

    //console.log("left:" + left + " top:" + top);
    //恢复原来的基础像素单位
    left *= seatWidth;
    top *= seatHeight;

    if (!seat_isEmpty(left, top)) {
        alert("位置重叠");
    }
    return { x: left, y: top };
}

//检测当前位置是否为空
function seat_isEmpty(left, top) {
    //避免座位重叠
    console.log("数组长度:" + seats.length);
    for (i in seats) {
        var tempseat = seats[i];
        //console.log(tempseat);
        var rect = tempseat.rect;
        if (rect.get('left') == left && rect.get('top') == top) {
            //console.log("出现位置重合");
            return false;
        }
    }
    return true;
}

//根据画布坐标 获取座位方位、坐标
function get_directionXY(left, top) {
    var direction = 0;
    var x = 0;
    var y = 0;

    //以座位尺寸为基本单位
    left = Math.floor(left / seatWidth);
    top = Math.floor(top / seatHeight);

    if (left < tableLeft_ColumnIndex) {
        direction = 1;
        x = tableLeft_ColumnIndex - left;
        y = tableBottom_RowIndex - top;
    } else if (top < tableTop_RowIndex) {
        direction = 2;
        x = left - tableLeft_ColumnIndex + 1;
        y = tableTop_RowIndex - top;
    } else if (left >= tableRight_ColumnIndex) {
        direction = 3;
        x = left - tableRight_ColumnIndex + 1;
        y = tableBottom_RowIndex - top;
    } else if (top >= tableBottom_RowIndex) {
        direction = 4;
        x = left - tableLeft_ColumnIndex + 1;
        y = top - tableBottom_RowIndex + 1;
    }

    return {
        direction: direction,
        x: x,
        y: y
    };
}

//当座位移动时，修改对应的direction、x、y
function set_seatDirectionXY(rect, seats) {
    var dirctionXY = get_directionXY(rect.left, rect.top);
    for (var i = 0; i < seats.length; i++) {
        if (seats[i].rect == rect) {
            seats[i].direction = dirctionXY.direction;
            seats[i].x = dirctionXY.x;
            seats[i].y = dirctionXY.y;
        }
    }
}

//在控制台显示当前seat参数
function console_log_seatStruct(message) {
    console.log(message);
    console.log(seat);
}

//窗口坐标转为cancvs坐标
function Pos_win2canvas(_x, _y) {
    var _box = _canvas.getBoundingClientRect();//_canvas元素相对于浏览器原点的坐标
    var x = _x - _box.left;
    var y = _y - _box.top;
    return {
        x: Math.floor(x),
        y: Math.floor(y)
    }
}

//将信息插入option
function insertOption(id, name, isSet, direction, x, y) { //参会人员id,名称name
    var delegateInfo = "<option value='" + id +
        "' isSet='" + isSet +
        "' direction='" + direction +
        "' x='" + x +
        "' y='" + y +
        "'>" + name +
        "</option>";
    $("#delegateInfos").append(delegateInfo);
}

//从data中获取数据，设置seat
function setSeat(data) {
    var id = data.id;
    var name = data.name;
    var isSet = data.isSet;
    var direction = data.direction;
    var x = data.x;
    var y = data.y;

    seat = new seatClass(id, name, isSet, direction, x, y);
}

//获取数据数组
function get_datas() {
    var options = $("#delegateInfos option");

    options.each(function () {
        var option = $(this);
        datas.push({
            id: option.val(),
            name: option.text(),
            isSet: option.attr("isSet"),
            direction: option.attr("direction"),
            x: option.attr("x"),
            y: option.attr("y")
        });
    });
}

//全局数据初始化//以座位为单位的 桌子宽、高
function init(_tableWidth_NumColumns, _tableHeight_NumRows) {
    //声明存储座位的数组
    seats = new Array();

    //声明座位
    seat = undefined;

    //定义座位在画布中的尺寸
    seatWidth = 80;
    seatHeight = 30;
    //根据画布大小、座位大小，定义座位在画布中的行数、列数（包括桌子）
    seatInCanvas_NumColumns = width / seatWidth;
    seatInCanvas_NumRows = height / seatHeight;
    //console.log(seatInCanvas_NumColumns + "," + seatInCanvas_NumRows);

    //根据桌子尺寸、画布大小，定义桌子在以座位尺寸为基础的范围(坐标以左上角为原点进行计算）
    tableLeft_ColumnIndex = Math.ceil((seatInCanvas_NumColumns - _tableWidth_NumColumns) / 2);
    tableTop_RowIndex = Math.ceil((seatInCanvas_NumRows - _tableHeight_NumRows) / 2);

    tableWidth_NumColumns = _tableWidth_NumColumns;
    tableHeight_NumRows = _tableHeight_NumRows;

    tableRight_ColumnIndex = tableLeft_ColumnIndex + tableWidth_NumColumns;
    tableBottom_RowIndex = tableTop_RowIndex + tableHeight_NumRows;

    //console.log(tableLeft_Column + "," + tableRight_Column + "," + tableTop_Row + "," + tableBottom_Row);

    //定义桌子参数
    tableLeft = (tableLeft_ColumnIndex) * seatWidth;
    tableTop = (tableTop_RowIndex) * seatHeight;
    tableWidth = tableWidth_NumColumns * seatWidth;
    tableHeight = tableHeight_NumRows * seatHeight;

    //画布清除
    canvas.clear();
    //画布初始化

    draw_grid();
    draw_tableBorder();

    //设置滑动条位置
    setCanvas_scroll();
}

//###################################################################
//绑定函数

$(function () {
    canvas.on({
        'object:move': function (e) {
            e.target.opacity = 0.5;
        },
        'object:modified': function (e) {
            //console.log(e.target);
            e.target.opacity = 1;

            var rect = e.target;
            //物体位置修正
            var position = position_change(rect.get('left'), rect.get('top'));
            //修正seats数组中相应的座位方位、x、y
            set_seatDirectionXY(rect, seats);
            //console.log("物体移动后，seats");
            //console.log(seats);
            rect.set({ left: position.x, top: position.y });

            canvas.renderAll();
        }
    })
})

//定义select中选项的点击事件
$(function () {
    $(document).on("change", "#delegateInfos", function () {
        var option = $("#delegateInfos option:checked");

        // console.log(option);
        seat = new seatClass(option.attr("value"), option.text());
    })
})


$(function () {
    canvas.on({
        'mouse:up': function (e) {
            e = (e || event).e;//获取鼠标点击事件
            //console.log(e);

            //当左键点击画布，且当前设置了人员时，绘制座位
            if (seat != undefined && e.button == 0) {
                //console.log("鼠标左键");

                //获取canvas坐标
                //console.log(e.clientX + "," + e.clientY);
                var position = Pos_win2canvas(e.clientX, e.clientY);

                //console.log("修正前");
                //console.log(position);

                //position修正
                position = position_change(position.x, position.y);
                //console.log("修正后");
                //console.log(position);

                console.log("seat.name:");
                console.log(seat.name);
                rect = make_LabeledRect(position.x, position.y, seatWidth, seatHeight, seat.name, '#faa', true);

                canvas.add(rect);

                //获取seat方位、x、y
                var directionXY = get_directionXY(position.x, position.y);
                seat.rect = rect;
                seat.direction = directionXY.direction;
                seat.x = directionXY.x;
                seat.y = directionXY.y;

                //console_log_seatStruct("新增的seat");

                seats.push(seat);
                seat = undefined;

                //console.log(seats);

                //移除 对应的option
                $("#delegateInfos option:checked").remove();
            } else if (e.button == 2) {
                //获取当前活动对象
                var activeRect = canvas.getActiveObject();
                if (activeRect && activeRect != table) {
                    //console.log("鼠标右键");

                    //遍历seats位置数组
                    //console.log("seats.length:" + seats.length);
                    for (var key in seats) {
                        //寻找当前绘制对象对应的 座位
                        if (seats[key].rect == activeRect) {
                            //console.log(seats[key].rect);
                            seat = seats[key];

                            //删除seats字典中对应的对象
                            seats.splice(key, 1);
                            break;
                        }
                    }

                    //console.log("seats.length:" + seats.length);

                    if (seat == undefined) {
                        return;
                    }

                    //将选项恢复回select列表
                    insertOption(seat.id, seat.name);

                    seat = undefined;

                    canvas.getActiveObject().remove();
                }
            }
        }
    })
})

//定义 桌子长宽修改后的参数设置，及画布重画
$(function () {
    $(document).on("change", "#table_width", function (e) {

        var width = Number($("#table_width").val());
        var height = Number($("#table_height").val());

        if (width <= 0 || height <= 0) {
            return;
        }

        //重新初始化参数设置,画布重绘
        init(width, height);
    });
    $(document).on("change", "#table_height", function (e) {

        var width = Number($("#table_width").val());
        var height = Number($("#table_height").val());

        if (width <= 0 || height <= 0) {
            return;
        }

        //重新初始化参数设置,画布重绘
        init(width, height);
    });
})

//###################################################################
//载入时调用
document.body.oncontextmenu = function () {
    //return false;
}

//调节滑动条位置 \ //滚动条滑到中间
function setCanvas_scroll() {
    var divWidth = parseInt($(".seatArrange-box").css('width'));
    var divHeight = parseInt($(".seatArrange-box").css('height'));

    $("div").animate({ "scrollLeft": divWidth / 2 * 3 + "px" }, 1000);
    $("div").animate({ "scrollTop": divHeight / 2 + "px" }, 1000);
};

$(function () {
    //获取数据
    get_datas();

    tableWidth_NumColumns = $("#tableWidth_NumColumns").val();
    tableHeight_NumRows = $("#tableHeight_NumRows").val();

    //初始化全局参数
    init();
})