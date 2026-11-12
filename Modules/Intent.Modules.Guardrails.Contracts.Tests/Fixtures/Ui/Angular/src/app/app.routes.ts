import { Routes } from '@angular/router';
import { authGuard } from './auth/auth.guard';
import { BrandListPageComponent } from './pages/brand/brand-list-page/brand-list-page.component';
import { CustomerAddPageComponent } from './pages/customer/customer-add-page/customer-add-page.component';
import { CustomerEditPageComponent } from './pages/customer/customer-edit-page/customer-edit-page.component';
import { CustomerListPageComponent } from './pages/customer/customer-list-page/customer-list-page.component';
import { HomeComponent } from './pages/home/home.component';

export const routes: Routes = [
  { path: 'brand/list', component: BrandListPageComponent },
  { path: 'customer/add', component: CustomerAddPageComponent, canActivate: [authGuard], data: { roles: ['Admin', 'Sales'] } },
  { path: 'customer/edit/:customerId', component: CustomerEditPageComponent, canActivate: [authGuard] },
  { path: 'customer-list', component: CustomerListPageComponent },
  { path: '', component: HomeComponent },
];
