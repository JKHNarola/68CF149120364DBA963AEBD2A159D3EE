import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AccountService } from '../../services/account.service';
import { ApiService } from '../../services/apiservice';
import { AuthService } from '../../services/app.auth.service';
import { ToastrService } from 'ngx-toastr';
import { Router, ActivatedRoute } from '@angular/router';
import { SetPasswordModel } from '../../models/account/setpassword.model';
import { pageSlideUpAnimation } from '../../misc/page.animation';

@Component({
    selector: 'set-password',
    templateUrl: './setpassword.component.html',
    styleUrls: ['./setpassword.component.scss'],
    providers: [ApiService, AccountService],
    animations: [pageSlideUpAnimation]
})
export class SetPasswordComponent implements OnInit {
    setpasswordForm: FormGroup;
    loading = false;
    isUsernameLoading = false;
    email: string;
    code: string;

    constructor(
        private route: ActivatedRoute,
        private accountService: AccountService,
        private authService: AuthService,
        private formBuilder: FormBuilder,
        private toastrService: ToastrService,
        private router: Router) {
        this.email = this.route.snapshot.queryParams.email;
        this.code = this.route.snapshot.queryParams.code;

        if (!this.email || !this.code) {
            this.router.navigate(['/home']);
        }
    }

    ngOnInit(): void {
        this.setpasswordForm = this.formBuilder.group({
            userName: ['', [Validators.required, Validators.maxLength(30)]],
            password: ['', [Validators.required, Validators.minLength(8)]]
        });
    }

    get f() { return this.setpasswordForm.controls; }

    onSetPassword() {
        this.loading = true;

        if (this.setpasswordForm.invalid) {
            this.loading = false;
            return;
        }

        let model = new SetPasswordModel();
        model.email = this.email;
        model.code = this.code;
        model.userName = this.setpasswordForm.value.userName;
        model.password = this.setpasswordForm.value.password;

        this.accountService.setPassword(model).subscribe(
            res => {
                if (res.status === 1) {
                    this.authService.setCurrentUser(res.jsonData);
                    this.toastrService.success("Your username & password are successfully set.");
                    this.router.navigate(['/home']);
                }
                else if (res.status === 2) {
                    this.toastrService.error("The code for setting username & password is seems to be invalid or exipred.");
                }
                this.setpasswordForm.reset();
                this.loading = false;
            },
            err => {
                this.loading = false;
            });
    }

    checkUserNameExists(userName: any) {
        this.isUsernameLoading = true;
        this.accountService.checkUserNameExists(userName.value).subscribe(
            res => {
                if (res.status === 1) {
                    this.setpasswordForm.controls.userName.setErrors({
                        exists: "Username is already taken"
                    })
                }
                this.isUsernameLoading = false;
            },
            err => {
                this.isUsernameLoading = false;
            });
    }
}
