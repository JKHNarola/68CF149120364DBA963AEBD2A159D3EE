import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { ApiRes } from "../models/apires.model";
import { BaseApiService } from "./baseapiservice";
import { LoginModel } from "../models/account/login.model";
import { RegisterModel } from "../models/account/register.model";
import { KeyValuePair } from "../models/keyvalue.model";
import { SetPasswordModel } from "../models/account/setpassword.model";
import { ResetPasswordModel } from "../models/account/resetpassword.model";
import { ChangePasswordModel } from "../models/account/changepassword.model";
import { HttpClient } from "@angular/common/http";

@Injectable()
export class AccountService extends BaseApiService {
    constructor(_httpClient: HttpClient) {
        super(_httpClient);
    }

    public login(username: string, password: string): Observable<ApiRes> {
        let model = new LoginModel();
        model.userName = username;
        model.password = password;
        model.rememberMe = false;

        return this.postWithoutAuth("/api/account/login", model);
    }

    public register(model: RegisterModel): Observable<ApiRes> {
        return this.postWithoutAuth("/api/account/register", model);
    }

    public confirmEmail(email: string, code: string): Observable<ApiRes> {
        let pairs = new Array<KeyValuePair>();
        let emailPair = new KeyValuePair();
        emailPair.key = "email";
        emailPair.value = email;
        pairs.push(emailPair);
        let codePair = new KeyValuePair();
        codePair.key = "code";
        codePair.value = code;
        pairs.push(codePair);

        return this.getByParamsWithoutAuth("/api/account/confirmemail", pairs);
    }

    public setPassword(model: SetPasswordModel): Observable<ApiRes> {
        return this.postWithoutAuth("/api/account/setpassword", model);
    }

    public checkUserNameExists(username: string): Observable<ApiRes> {
        let pair = new KeyValuePair();
        pair.key = "userName";
        pair.value = username;
        return this.getByParamsWithoutAuth("/api/account/check/usernameexist", [pair]);
    }

    public requestResetPassword(email: string): Observable<ApiRes> {
        let emailPair = new KeyValuePair();
        emailPair.key = "email";
        emailPair.value = email;

        return this.getByParamsWithoutAuth("/api/account/forgotpassword", [emailPair]);
    }

    public resetPassword(model: ResetPasswordModel): Observable<ApiRes> {
        return this.postWithoutAuth("/api/account/resetpassword", model);
    }

    public changePassword(model: ChangePasswordModel): Observable<ApiRes> {
        return this.post("/api/account/changepassword", model);
    }

    public logout(): Observable<ApiRes> {
        return this.get("/api/account/logout");
    }
}
