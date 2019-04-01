"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var app_enums_1 = require("./app.enums");
var Query = /** @class */ (function () {
    function Query(pageNo, pageSize) {
        this.pageNo = pageNo == null ? 1 : pageNo;
        this.pageSize = pageSize == null ? 10 : pageSize;
        this.whereClauseParts = [];
        this.sorts = [];
        this.extras = new Dictionary();
    }
    Query.prototype.addStartBracket = function (logic) {
        if (!logic)
            logic = app_enums_1.Logic.And;
        this.whereClauseParts.push(new ConditionPart(true, false, null, null, null, logic));
    };
    Query.prototype.addEndBracket = function () {
        this.whereClauseParts.push(new ConditionPart(false, true, null, null, null, null));
    };
    Query.prototype.addCondition = function (columnName, value, op, logic) {
        if (op == null)
            op = app_enums_1.Operator.Eq;
        if (!logic)
            logic = app_enums_1.Logic.And;
        this.whereClauseParts.push(new ConditionPart(false, false, columnName, op, value, logic));
    };
    Query.prototype.addConditionIsNull = function (columnName, logic) {
        this.whereClauseParts.push(new ConditionPart(false, false, columnName, app_enums_1.Operator.IsNull, null, logic));
    };
    Query.prototype.addConditionIsNotNull = function (columnName, logic) {
        if (!logic)
            logic = app_enums_1.Logic.And;
        this.whereClauseParts.push(new ConditionPart(false, false, columnName, app_enums_1.Operator.IsNotNull, null, logic));
    };
    Query.prototype.addConditionIsEmpty = function (columnName, logic) {
        if (!logic)
            logic = app_enums_1.Logic.And;
        this.whereClauseParts.push(new ConditionPart(false, false, columnName, app_enums_1.Operator.IsEmpty, null, logic));
    };
    Query.prototype.addConditionIsNotEmpty = function (columnName, logic) {
        if (!logic)
            logic = app_enums_1.Logic.And;
        this.whereClauseParts.push(new ConditionPart(false, false, columnName, app_enums_1.Operator.IsNotEmpty, null, logic));
    };
    Query.prototype.addSort = function (sort) {
        if (sort && sort.columnName) {
            var filtered = this.sorts.filter(function (x) { return x.columnName.toLowerCase() == sort.columnName.toLowerCase(); });
            if (filtered != null && filtered.length >= 1)
                filtered[0].direction = sort.direction;
            else
                this.sorts.push(sort);
        }
    };
    Query.prototype.addExtra = function (key, value) {
        this.extras.add(key, value);
    };
    Query.prototype.addAggregator = function (columnName, aggregate) {
        var filtered = this.aggregates.filter(function (x) { return x.columnName.toLowerCase() == columnName.toLowerCase(); });
        if (filtered != null && filtered.length >= 1)
            filtered[0].aggregate = aggregate;
        else
            this.aggregates.push(new Aggregator(columnName, aggregate));
    };
    return Query;
}());
exports.Query = Query;
var Sort = /** @class */ (function () {
    function Sort() {
        this.columnName = null;
        this.direction = app_enums_1.SortOrder.Asc;
    }
    return Sort;
}());
exports.Sort = Sort;
var ConditionPart = /** @class */ (function () {
    function ConditionPart(isStartBracket, isEndBracket, columnName, operator, value, logic) {
        this.isEndBracket = isEndBracket;
        this.isStartBracket = isStartBracket;
        this.columnName = columnName;
        this.operator = operator;
        this.value = value;
        this.logic = logic;
    }
    return ConditionPart;
}());
exports.ConditionPart = ConditionPart;
var Aggregator = /** @class */ (function () {
    function Aggregator(columnName, aggregate) {
        this.columnName = columnName;
        this.aggregate = aggregate;
    }
    return Aggregator;
}());
exports.Aggregator = Aggregator;
var Dictionary = /** @class */ (function () {
    function Dictionary(init) {
        if (init) {
            for (var x = 0; x < init.length; x++) {
                this[init[x].key] = init[x].value;
            }
        }
    }
    Dictionary.prototype.add = function (key, value) {
        this[key] = value;
    };
    Dictionary.prototype.remove = function (key) {
        delete this[key];
    };
    Dictionary.prototype.containsKey = function (key) {
        if (typeof this[key] === "undefined")
            return false;
        return true;
    };
    return Dictionary;
}());
exports.Dictionary = Dictionary;
var Paginator = /** @class */ (function () {
    function Paginator(pageNo, pageSize, maxSize) {
        this.pageNo = pageNo || pageNo == 0 ? pageNo : 1;
        this.pageSize = pageSize || pageSize == 0 ? pageSize : 10;
        this.maxSize = maxSize ? maxSize : 10;
    }
    return Paginator;
}());
exports.Paginator = Paginator;
