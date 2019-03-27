import { SortOrder, Operator, Logic } from "./app.enums";

export class Query {
    public whereClauseParts: Array<ConditionPart>;
    public extras: Dictionary<any>;
    public sorts: Array<Sort>;
    public aggregates: Array<Aggregator>;
    public pageNo: number;
    public pageSize: number;

    constructor(pageNo?: number, pageSize?: number) {
        this.pageNo = pageNo == null ? 1 : pageNo;
        this.pageSize = pageSize == null ? 10 : pageSize;
        this.whereClauseParts = [];
        this.sorts = [];
        this.extras = new Dictionary<any>();
    }

    public addStartBracket() {
        this.whereClauseParts.push(new ConditionPart(true, false, null, null, null, null));
    }

    public addEndBracket() {
        this.whereClauseParts.push(new ConditionPart(false, true, null, null, null, null));
    }

    public addCondition(columnName: string, value: any, op?: Operator) {
        if (op == null) op = Operator.Eq;
        this.whereClauseParts.push(new ConditionPart(false, false, columnName, op, value, null));
    }

    public addConditionIsNull(columnName: string) {
        this.whereClauseParts.push(new ConditionPart(false, false, columnName, Operator.IsNull, null, null));
    }

    public addConditionIsNotNull(columnName: string) {
        this.whereClauseParts.push(new ConditionPart(false, false, columnName, Operator.IsNotNull, null, null));
    }

    public addConditionIsEmpty(columnName: string) {
        this.whereClauseParts.push(new ConditionPart(false, false, columnName, Operator.IsEmpty, null, null));
    }

    public addConditionIsNotEmpty(columnName: string) {
        this.whereClauseParts.push(new ConditionPart(false, false, columnName, Operator.IsNotEmpty, null, null));
    }

    public addLogic(logic?: Logic) {
        if (logic == null) logic = Logic.And;
        this.whereClauseParts.push(new ConditionPart(false, false, null, null, null, logic));
    }

    public addAnd() {
        this.whereClauseParts.push(new ConditionPart(false, false, null, null, null, Logic.And));
    }

    public addOr() {
        this.whereClauseParts.push(new ConditionPart(false, false, null, null, null, Logic.Or));
    }

    public addSort(columnName: string, direction: SortOrder) {
        let filtered = this.sorts.filter(x => x.columnName.toLowerCase() == columnName.toLowerCase());
        if (filtered != null && filtered.length >= 1) filtered[0].direction = direction;
        else
            this.sorts.push(new Sort(columnName, direction));
    }

    public addExtra(key: string, value: any) {
        this.extras.add(key, value);
    }

    public addAggregator(columnName: string, aggregate: string) {
        let filtered = this.aggregates.filter(x => x.columnName.toLowerCase() == columnName.toLowerCase());
        if (filtered != null && filtered.length >= 1) filtered[0].aggregate = aggregate;
        else
            this.aggregates.push(new Aggregator(columnName, aggregate));
    }
}

export class Sort {
    public columnName: string;
    public direction: SortOrder;
    constructor(columnName: string, direction: SortOrder) {
        this.columnName = columnName;
        this.direction = direction;
    }
}

export class ConditionPart {
    public isStartBracket: boolean;
    public isEndBracket: boolean;
    public columnName: string;
    public operator?: Operator;
    public value: any;
    public logic?: Logic;
    constructor(
        isStartBracket: boolean,
        isEndBracket: boolean,
        columnName: string,
        operator: Operator,
        value: any,
        logic: Logic) {
        this.isEndBracket = isEndBracket;
        this.isStartBracket = isStartBracket;
        this.columnName = columnName;
        this.operator = operator;
        this.value = value;
        this.logic = logic;
    }
}

export class Aggregator {
    public columnName: string;
    public aggregate: string;
    constructor(columnName: string, aggregate: string) {
        this.columnName = columnName;
        this.aggregate = aggregate;
    }
}

export class Dictionary<T> {
    constructor(init?: { key: string; value: T; }[]) {
        if (init) {
            for (var x = 0; x < init.length; x++) {
                this[init[x].key] = init[x].value;
            }
        }
    }

    add(key: string, value: T) {
        this[key] = value;
    }

    remove(key: string) {
        delete this[key];
    }

    containsKey(key: string) {
        if (typeof this[key] === "undefined") return false;
        return true;
    }
}
