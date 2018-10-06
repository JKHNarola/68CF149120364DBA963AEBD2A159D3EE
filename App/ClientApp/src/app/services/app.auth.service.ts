import { Headers, RequestOptions } from '@angular/http';
import { CurrUser } from "../models/curruser.model";
import { Injectable } from '@angular/core';
import * as jwt_decode from "jwt-decode";
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable()
export class AuthService {

    private currUserSetSource = new BehaviorSubject<CurrUser>(null);
    onCurrUserSet: Observable<CurrUser> = this.currUserSetSource.asObservable();

    private currUserRemovedSource = new BehaviorSubject<boolean>(false);
    onCurrUserRemoved: Observable<boolean> = this.currUserRemovedSource.asObservable();

    public getAuthHeader() {
        let currentUser = JSON.parse(String(localStorage.getItem('currentUser')));
        if (currentUser && currentUser.token) {
            let header = new Headers({ 'Authorization': 'Bearer ' + currentUser.token });
            return new RequestOptions({ headers: header });
        }
        else {
            return null;
        }
    }

    public isUserLoggedIn() {
        let currUser = this.getCurrentUser();
        if (currUser && currUser.token) {
            return true;
        }
        else {
            return false;
        }
    }

    public getToken() {
        try {
            let currentUser = JSON.parse(String(localStorage.getItem('currentUser')));
            if (currentUser && currentUser.token) {
                return currentUser.token;
            }
            else {
                return null;
            }
        }
        catch (ex) {
            return null;
        }
    }

    public getCurrentUserId() {
        try {
            let currentUser = JSON.parse(String(localStorage.getItem('currentUser')));
            if (currentUser && currentUser.id) {
                return currentUser.id;
            }
            else {
                return null;
            }
        }
        catch (ex) {
            return null;
        }
    }

    public getCurrentUser(): CurrUser {
        try {
            return <CurrUser>(JSON.parse(String(localStorage.getItem('currentUser'))));
        }
        catch (ex) {
            return null;
        }
    }

    public setCurrentUser(model: CurrUser) {
        localStorage.setItem('currentUser', JSON.stringify(model));
        this.currUserSetSource.next(model);
    }

    public removeCurrentUser() {
        localStorage.removeItem('currentUser');
        this.currUserRemovedSource.next(true);
    }

    public getTokenExpirationTime(): Date {
        try {
            let token = this.getToken();
            if (token != null) {
                let decodedToken = jwt_decode(token);
                if (decodedToken)
                    return new Date(decodedToken.exp * 1000);
            }
        }
        catch (Error) {
        }
        return null;
    }

    public isTokenExpired(): boolean {
        try {
            let exp = this.getTokenExpirationTime();
            if (exp != null && (exp > new Date()))
                return false;
            else
                return true;
        }
        catch (Error) {
        }
        return true;
    }

}
