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

        ctx.font = this.font || '20px Helvetica';
        ctx.fillStyle = this.fillStyle || 'black';
        ctx.fillText(this.label, -this.width / 2, -this.height / 2 + 20);
    }
});

//###################################################################
//定义全局变量
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

//定义座位
function SeatClass(id, name, seatIndex, userLevel) {
    this.id = id;
    this.name = name;
    this.seatIndex = seatIndex;
    this.userLevel = userLevel;
    this.rect = undefined;

    this.reset = function () {
        this.seatIndex = 0;
        this.rect = undefined;
    }
}
//人员座位安排数据数组
var seatInfos = new Array();

//定义空座位
function FreeSeatClass(seatIndex,rect){
    this.seatIndex = seatIndex;
    this.rect = rect;
}
//声明空座位数组
var freeSeats = new Array();

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
//定义长桌每一边可容纳人数
var table_upNum;
var table_downNum;
var table_leftNum;
var table_rightNum;

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
function make_LabeledRect(_left, _top, _width, _height, _text, _selectable, _fill, _fillStyle, _font) {
    _fill = _fill || '#faa';
    _fillStyle = _fillStyle || 'black';
    return new LabeledRect({
        left: _left,
        top: _top,
        width: _width,
        height: _height,
        label: _text,
        selectable: _selectable,// 是否可选
        hasControls: false, //不允许修改形状
        hasBorders: false,// 取消选择时，显示边界
        fill: _fill,
        fillStyle: _fillStyle,
        font: _font || '20px Helvetica'
    });
}

//返回矩形对象
function make_Rect(_left, _top, _width, _height, _fill, _selectable) {
    console.log(_selectable);

    _fill = _fill || 'white';
    return new LabeledRect({
        left: _left,
        top: _top,
        width: _width,
        height: _height,
        selectable: _selectable,// 是否可选
        hasControls: false, //不允许修改形状
        hasBorders: false,// 取消选择时，显示边界
        fill: _fill
    });
}


//在画布中绘制全部座位的网格线
function draw_grid() {
    var fill = 'green';
    var seatSum = table_upNum+table_downNum+table_leftNum+table_rightNum;
    for(var i = 1 ; i <= seatSum;i++){
        var position = get_left_top(i);
        var rect = (make_LabeledRect(position.left, position.top, seatWidth, seatHeight,i,false, fill, 'white'));
        canvas.add(rect);
        freeSeats.push(new FreeSeatClass(i,rect));
    }
}

//绘制桌子
function draw_table() {
    canvas.add(make_Rect(tableLeft, tableTop, tableWidth, tableHeight, 'red', false));
}

//画布中，座位图形坐标修正// 避免重叠，符合网格
function position_change(left, top) { // x,y

    //以座位尺寸为基本单位
    left = Math.floor(left / seatWidth);
    top = Math.floor(top / seatHeight);

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
function seat_isEmpty(seatIndex) {
    //避免座位重叠
    //console.log("数组长度:" + seatInfos.length);
    for (var i = 0 ; i < seatInfos.length;i++) {
        var tempseat = seatInfos[i];
        //console.log(tempseat);
        if (tempseat.seatIndex==seatIndex) {
            //console.log("出现位置重合");
            return false;
        }
    }
    return true;
}

//根据画布坐标 获取座位编号、修正后的画布坐标
function get_seatIndex_left_top(left, top) {
    var seatIndex = 0;

    //转换为 以座位为单位的坐标
    left = Math.floor(left / seatWidth);
    top = Math.floor(top / seatHeight);

    //从桌子外修正到桌子周围
    left = left <= (tableLeft_ColumnIndex - 1) ?
        tableLeft_ColumnIndex - 1 :
        (left >= tableRight_ColumnIndex ?
        tableRight_ColumnIndex : left);
    top = top <= (tableTop_RowIndex - 1) ?
        tableTop_RowIndex - 1 :
        (top >= tableBottom_RowIndex ?
        tableBottom_RowIndex : top);

    //从桌子内修正到桌子周围
    //修正到桌子左边
    left = (left < tableRight_ColumnIndex && left >= tableLeft_ColumnIndex
        && top < tableBottom_RowIndex && top >= tableTop_RowIndex) ?
        (left < (tableRight_ColumnIndex + tableLeft_ColumnIndex) / 2 ?
        tableLeft_ColumnIndex - 1 : tableRight_ColumnIndex) :
        left;

    if ( top == tableTop_RowIndex - 1) {
        //上边
        if (left == tableLeft_ColumnIndex - 1) {
            left = tableLeft_ColumnIndex;
        }
        if (left == tableRight_ColumnIndex) {
            left = tableRight_ColumnIndex - 1;
        }
        var index = left - tableLeft_ColumnIndex + 1;

        if (index > table_upNum) {
            index = table_upNum;
        }
        left = tableLeft_ColumnIndex + index -1;
        //恢复以像素为单位的坐标
        left *= seatWidth;
        top *=seatHeight;

        //遍历座位，设置对应的seatIndex
        for(var i = 0 ; i < freeSeats.length;i++){
            if(freeSeats[i].rect.get('left')==left 
                && freeSeats[i].rect.get('top')==top){
                seatIndex = freeSeats[i].seatIndex;
            }
        }
    } else if (top == tableBottom_RowIndex) {
        //下边
        if (left == tableLeft_ColumnIndex - 1) {
            left = tableLeft_ColumnIndex;
        }
        if (left == tableRight_ColumnIndex) {
            left = tableRight_ColumnIndex - 1;
        }

        var index = left - tableLeft_ColumnIndex + 1;

        if (index > table_downNum) {
            index = table_downNum;
        }
        left = tableLeft_ColumnIndex + index -1;
        //恢复以像素为单位的坐标
        left *= seatWidth;
        top *=seatHeight;

        //遍历座位，设置对应的seatIndex
        for(var i = 0 ; i < freeSeats.length;i++){
            if(freeSeats[i].rect.get('left')==left 
                && freeSeats[i].rect.get('top')==top){
                seatIndex = freeSeats[i].seatIndex;
            }
        }
    } else if (left == tableLeft_ColumnIndex - 1) {
        //左边

        var index = top - tableTop_RowIndex + 1;

        if (index > table_leftNum) {
            index = table_leftNum;
        }
        top = tableTop_RowIndex + index -1;
        //恢复以像素为单位的坐标
        left *= seatWidth;
        top *=seatHeight;

        //遍历座位，设置对应的seatIndex
        for(var i = 0 ; i < freeSeats.length;i++){
            if(freeSeats[i].rect.get('left')==left 
                && freeSeats[i].rect.get('top')==top){
                seatIndex = freeSeats[i].seatIndex;
            }
        }
    } else if (left == tableRight_ColumnIndex) {
        //右边

        var index = top - tableTop_RowIndex + 1;

        if (index > table_rightNum) {
            index = table_rightNum;
        }
        top = tableTop_RowIndex + index -1;
        //恢复以像素为单位的坐标
        left *= seatWidth;
        top *=seatHeight;

        //遍历座位，设置对应的seatIndex
        for(var i = 0 ; i < freeSeats.length;i++){
            if(freeSeats[i].rect.get('left')==left 
                && freeSeats[i].rect.get('top')==top){
                seatIndex = freeSeats[i].seatIndex;
            }
        }
    }
    return {
        seatIndex : seatIndex,
        left : left,
        top : top
    };
}

//修正对应rect对象的seatInfos中的seatIndex
function change_seatIndex(rect, seatIndex) {
    for (var i = 0 ; i < seatInfos.length; i++) {
        if (seatInfos[i].seatIndex != 0 && seatInfos[i].rect == rect) {
            seatInfos[i].rect = rect;
            seatInfos[i].seatIndex = seatIndex;
        }
    }
}

//给定方位和编号，以左下建立轴
function get_left_top_via(direction, index) { //index >=1
    var left;
    var top;
    switch (direction) {
        case 'top'://top
            left = (tableLeft_ColumnIndex + index - 1 ) * seatWidth;
            top = (tableTop_RowIndex - 1) * seatHeight;
            break;
        case 'left'://left
            left = (tableLeft_ColumnIndex - 1) * seatWidth;
            top = (tableTop_RowIndex + index - 1) * seatHeight;
            break;
        case 'right'://right
            left = tableRight_ColumnIndex * seatWidth;
            top = (tableTop_RowIndex + index - 1) * seatHeight;
            break;
        case 'down'://down
            left = (tableLeft_ColumnIndex + index - 1 ) * seatWidth;
            top = tableBottom_RowIndex * seatHeight;
            break;
    }
    return {
        left: left,
        top: top
    };
}

//根据给定座位编号,及桌子每边可容纳人数、画布大小、桌子大小，获取画布坐标
function get_left_top(seatIndex) { //seatIndex >= 1
    //对于座位的计算，以上边开始，即
    //  右卑       3   1   2       左尊
    //         5   0   0   0   4
    //         7   0   0   0   6
    //         8   0   0   0   
    //            10   9   
    
    if (seatIndex <= table_upNum
        && seatIndex >= 1 )
    {//从上边开始//以上边为主
        if (seatIndex == table_upNum) {
            var index = 1;
            return get_left_top_via('top', index);
        }
        if (table_upNum % 2 == 0) {
            //偶数
            
            //获取中间值,
            var center = (table_upNum / 2 + 1);
            //如果seatIndex==1
            if (seatIndex == 1) {
                return get_left_top_via('top', center);
            }

            //面向屏幕时 左边为偶数，右边为奇数
            if (seatIndex % 2 == 1) {
                var count = (seatIndex - 1) / 2;
                var index = center + count;
                return get_left_top_via('top', index);
            } else {
                var count = seatIndex / 2;
                var index = center - count;
                return get_left_top_via('top', index);
            }
        } else {
            //获取中间值,向上取整
            var center = Math.ceil(table_upNum / 2);
            //如果seatIndex==1
            if (seatIndex == 1) {
                return get_left_top_via('top', center);
            }

            //面向屏幕时 右边为偶数，左边为奇数
            if (seatIndex % 2 == 1) {
                var count = (seatIndex - 1) / 2;
                var index = center - count;
                return get_left_top_via('top', index);
            } else {
                var count = seatIndex / 2;
                var index = center + count;
                return get_left_top_via('top', index);
            }
        }
    }
    else if (seatIndex > (table_upNum + table_leftNum + table_rightNum)
        && seatIndex <= (table_upNum + table_leftNum + table_rightNum + table_downNum))
    {//判断是否位于下边
        var downIndex = seatIndex - (table_upNum + table_leftNum + table_rightNum);

        if (downIndex == 1) {
            //位置为下边最后一个
            var index = table_downNum;
            return get_left_top_via('down', index);
        }

        //面向屏幕时 左边为偶数，右边为奇数
        if (downIndex % 2 == 0) {
            var index = (downIndex) / 2;
            return get_left_top_via('down', index);
        } else {
            var index = table_downNum - (downIndex - 1) / 2;
            return get_left_top_via('down', index);
        }
    } else if(seatIndex <= ((table_upNum + table_leftNum + table_rightNum))
        && seatIndex > table_upNum){
        //在左边或者右边
        var lrIndex = seatIndex - table_upNum;

        //取两边中最小值
        var minValue = Math.min(table_leftNum, table_rightNum);

        if (lrIndex <= minValue * 2) {
            //lrIndex遵循 奇数在 right边 偶数在 left边
            if (lrIndex % 2 == 0) {
                var index = lrIndex / 2;
                return get_left_top_via('left', index);
            } else {
                var index = (lrIndex + 1) / 2;
                return get_left_top_via('right', index);
            }
        }

        if (minValue == table_leftNum) {
            //桌子右边还有剩余座位
            var index = (lrIndex - minValue);
            return get_left_top_via('right', index);
        } else {
            //桌子左边还有剩余
            var index = (lrIndex - minValue);
            return get_left_top_via('left', index);
        }
    } else {
        alert("座位编号不合理");
    }
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
function insertOption(id, name, seatIndex) { //参会人员id,名称name
    var delegateInfo = "<option value='" + id +
        "' seatIndex='" + seatIndex +
        "'>" + name +
        "</option>";
    $("#delegateInfos").append(delegateInfo);
}

//获取数据数组
function get_seatInfos() {

    var tempSeatInfos = new Array();

    var options = $("#seatInfos option");

    options.each(function () {
        var option = $(this);
        //console.log(option.val());
        //console.log(option.text());
        //console.log(option.attr("seatIndex"));
        //console.log(option.attr("userLevel"));
        tempSeatInfos.push(
            new SeatClass(
                option.val(),
                option.text(),
                option.attr("seatIndex"),
                option.attr("userLevel")));
    });
    return tempSeatInfos;
}

//设置长桌每边可容纳人数
function setTable_up_down_left_right() {
    table_upNum = parseInt($("#upNum").val());
    table_downNum = parseInt($("#downNum").val());
    table_leftNum = parseInt($("#leftNum").val());
    table_rightNum = parseInt($("#rightNum").val());
}

//全局数据初始化//以座位为单位的 桌子宽、高
function init() {

    //设置长桌每边可容纳人数
    setTable_up_down_left_right();

    //设置桌子 以座位尺寸为基础 的width height 取桌子对边可容纳人数的最大值
    tableWidth_NumColumns = Math.max(table_upNum, table_downNum);
    tableHeight_NumRows = Math.max(table_leftNum, table_rightNum);

    //设置存储座位信息的数组
    seatInfos = get_seatInfos();

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
    tableLeft_ColumnIndex = Math.ceil((seatInCanvas_NumColumns - tableWidth_NumColumns) / 2);
    tableTop_RowIndex = Math.ceil((seatInCanvas_NumRows - tableHeight_NumRows) / 2);

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
    //绘制桌子

    //绘制座位
    draw_grid();
    draw_table();

    //设置滑动条位置
    setCanvas_scroll();
}

//绘制座位信息
function draw_seat(left, top, text) {
    var seatRect = make_LabeledRect(left, top, seatWidth, seatHeight, text, true, 'black', 'white');

    canvas.add(seatRect);
    return seatRect;
}

//如果排座未进行，将自动排座
function autoSeat() {
    var isSet = 0;
    for (var i = 0 ; i < seatInfos.length; i++) {
        if (seatInfos[i].seatIndex != 0) {
            isSet = 1;
        }
    }

    var seatSum = (table_upNum + table_downNum + table_leftNum + table_rightNum);
    if (!isSet) {
        //之前未进行排序，将按照数据顺序进行排座
        var seatIndex = 1;
        for (var i = 0 ; i < seatInfos.length; i++) {
            if (seatIndex > seatSum) {
                alert("座位数量不足");
                break;
            }
            var position = get_left_top(seatIndex);

            seatInfos[i].seatIndex = seatIndex;

            seatInfos[i].rect = draw_seat(position.left, position.top,seatInfos[i].name);

            seatIndex++;
        }
    } else {
        //之前已排序，将按照之前的排序顺序进行排座
        for (var i = 0 ; i < seatInfos.length; i++) {
            if (seatInfos[i].seatIndex == 0) {//某些人员未安排
                continue;
            }
            if (seatInfos[i].seatIndex > freeSeats.length) {
                alert("编号越界");
                continue;
            }
            var position = get_left_top(seatInfos[i].seatIndex);
            seatInfos[i].rect = draw_seat(position.left, position.top, seatInfos[i].name);
        }
    }
}

//重置排座信息
function resetSeatInfos() {
    for (var i = 0 ; i < seatInfos.length; i++) {
        seatInfos[i].seatIndex = 0;
    }
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
            var positionLT = get_seatIndex_left_top(rect.get('left'), rect.get('top'));

            rect.set({ left: positionLT.left, top: positionLT.top });
            //console.log(seatInfos);
            //修正
            change_seatIndex(rect, positionLT.seatIndex);
            //console.log(seatInfos);
            canvas.renderAll();
        }
    })
})

//定义select中选项的点击事件
$(function () {
    $(document).on("click", "#seatInfos", function () {
        var option = $("#seatInfos option:checked");

        // console.log(option);
        seat = new SeatClass(option.attr("value"), option.text());

        console.log(seat);
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

                var positionLT = get_seatIndex_left_top(position.x,position.y);

                var rect = draw_seat(positionLT.left, positionLT.top, seat.name);
                for(var i = 0 ; i < seatInfos.length;i++){
                    if(seat.id==seatInfos[i].id){
                        seatInfos[i].seatIndex = positionLT.seatIndex;
                        if (seatInfos[i].rect != undefined) {
                            seatInfos[i].rect.remove();
                        }
                        seatInfos[i].rect = rect;
                    }
                }
                console.log(seatInfos);
                seat = undefined;
                //移除 对应的option
                $("#delegateInfos option:checked").remove();
            } else if (e.button == 2) {
                //获取当前活动对象
                var activeRect = canvas.getActiveObject();
                if (activeRect && activeRect != table) {
                    //console.log("鼠标右键");

                    //遍历seatinfos位置数组
                    for (var key in seatInfos) {
                        //寻找当前绘制对象对应的 座位
                        if (seatInfos[key].rect == activeRect) {
                            //console.log(seats[key].rect);
                            seat = seatInfos[key];
                            //重置seatInfos字典中对应的rect、seatIndex
                            seatInfos[key].seatIndex=0;

                            seatInfos[key].rect = undefined;
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

$(function () {
    $(document).on("click", "#reset", function (e) {
        init();
        resetSeatInfos();
        autoSeat();
    });
    $(document).on("click", "#byhand", function (e) {
        init();
        resetSeatInfos();
    });
    $(document).on("click", "#gobackCenter", function (e) {
        setCanvas_scroll();
    });
})

//保存座位安排信息
$(function () {
    $(document).on("click", "#save", function (e) {

        var datas = new Array();
        for (var i = 0 ; i < seatInfos.length; i++) {
            datas.push({
                delegateID: seatInfos[i].id,
                userName: seatInfos[i].name,
                deviceIndex: 0,//未使用
                seatIndex: seatInfos[i].seatIndex,
                userLevel: seatInfos[i].userLevel
            });
        }
        var str = JSON.stringify(datas);
        $.ajax({
            type: "POST",
            url: "/Delegate/SeatArrange_organizor",
            data: str,
            dataType: "json",
            headers: {
                "Content-Type": "application/json"
            },
            success: function (respond) {
                console.log(respond);
            }
        });
    });
})

//返回
$(function () {
    $(document).on("click", "#return", function (e) {
        var meetingID = $("#meetingID").val();
        window.location.href = "/Delegate/Index_organizor?meetingID=" + meetingID;
    });
})
//###################################################################
//载入时调用
document.body.oncontextmenu = function () {
    return false;
}

//调节滑动条位置 \ //滚动条滑到中间
function setCanvas_scroll() {
    var divWidth = parseInt($(".seatArrange-box").css('width'));
    var divHeight = parseInt($(".seatArrange-box").css('height'));

    $("div").animate({ "scrollLeft": divWidth / 2 * 3 + "px" }, 1000);
    $("div").animate({ "scrollTop": divHeight / 2 + "px" }, 1000);
};

$(function () {
    //初始化全局参数
    init();

    //自动排座
    autoSeat();
})
