import { Injectable } from "@angular/core";
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Observable } from "rxjs";
import { KeyValuePair } from "../models/keyvalue.model";
import { ApiRes } from "../models/apires.model";
import { AppConsts } from "../misc/app.consts";

@Injectable()
export class BaseApiService {
    private skipAuthHeaders = new HttpHeaders().set(AppConsts.interceptorSkipAuthHeader, '');

    constructor(private httpClient: HttpClient) {
    }

    private mapHttpParams(params: KeyValuePair[]): HttpParams {
        let httpParams = new HttpParams();
        params.forEach(x => {
            httpParams = httpParams.append(x.key, x.value);
        });

        return httpParams;
    }

    public get(url: string): Observable<ApiRes> {
        return this.httpClient.get<ApiRes>(url);
    }

    public getWithoutAuth(url: string): Observable<ApiRes> {
        return this.httpClient.get<ApiRes>(url, { headers: this.skipAuthHeaders });
    }

    public getByParams(url: string, params: KeyValuePair[]): Observable<ApiRes> {
        return this.httpClient.get<ApiRes>(url, { params: this.mapHttpParams(params) });
    }

    public getByParamsWithoutAuth(url: string, params: KeyValuePair[]): Observable<ApiRes> {
        return this.httpClient.get<ApiRes>(url, { params: this.mapHttpParams(params), headers: this.skipAuthHeaders });
    }

    public post<T>(url: string, data: T): Observable<ApiRes> {
        return this.httpClient.post<ApiRes>(url, data);
    }

    public postWithoutAuth<T>(url: string, data: T): Observable<ApiRes> {
        return this.httpClient.post<ApiRes>(url, data, { headers: this.skipAuthHeaders });
    }
}
