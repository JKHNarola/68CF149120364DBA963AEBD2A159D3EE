import { Component, OnInit } from "@angular/core";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { AccountService } from "../../services/account.service";
import { ApiService } from "../../services/apiservice";
import { AuthService } from "../../services/app.auth.service";
import { ToastrService } from "ngx-toastr";
import { Router } from "@angular/router";
import { pageSlideUpAnimation } from "../../misc/page.animation";
import { ChangePasswordModel } from "../../models/account/changepassword.model";

@Component({
    selector: "change-password",
    templateUrl: "./changepassword.component.html",
    providers: [ApiService, AccountService],
    animations: [pageSlideUpAnimation]
})
export class ChangePasswordComponent implements OnInit {
    changePasswordForm: FormGroup;
    loading = false;

    constructor(
        private accountService: AccountService,
        private authService: AuthService,
        private formBuilder: FormBuilder,
        private toastrService: ToastrService,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.changePasswordForm = this.formBuilder.group({
            newPassword: ["", [Validators.required, Validators.minLength(8)]],
            currentPassword: ["", [Validators.required]]
        });
    }

    get f() {
        return this.changePasswordForm.controls;
    }

    onChangePassword() {
        this.loading = true;

        if (this.changePasswordForm.invalid) {
            this.loading = false;
            return;
        }

        let model = new ChangePasswordModel();
        model.currentPassword = this.changePasswordForm.value.currentPassword;
        model.newPassword = this.changePasswordForm.value.newPassword;

        this.accountService.changePassword(model).subscribe(
            res => {
                if (res.status === 1) {
                    this.authService.setCurrentUser(res.jsonData);
                    this.toastrService.success(
                        "Your password is successfully changed."
                    );
                    this.router.navigate(["/home"]);
                    this.changePasswordForm.reset();
                } else if (res.status === 2) {
                    if (res.jsonData[0].code == "PasswordMismatch")
                        this.toastrService.error(
                            "Incorrect current password.<br>Please try again."
                        );
                    else {
                        let msg =
                            "New password must contain followings.<br><br>";
                        for (let x of res.jsonData) {
                            msg += x.description + "<br>";
                        }
                        msg += "<br>Please try again.";
                        this.toastrService.error(msg);
                    }
                }
                this.loading = false;
            },
            err => {
                this.loading = false;
            }
        );
    }
}
