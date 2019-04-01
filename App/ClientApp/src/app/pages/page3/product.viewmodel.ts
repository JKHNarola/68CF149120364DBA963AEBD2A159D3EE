export class ProductViewModel {
    productID: number;
    productName: string;
    supplierID?: number;
    supplierCompanyName: string;
    supplierContactName: string;
    supplierContactTitle: string;
    supplierAddress: string;
    categoryID?: number;
    categoryName: string;
    categoryPicture: Array<number>;
    quantityPerUnit: string;
    unitPrice?: number;
    unitsInStock?: number;
    unitsOnOrder?: number;
    reorderLevel?: number;
    discontinued: boolean;
}
