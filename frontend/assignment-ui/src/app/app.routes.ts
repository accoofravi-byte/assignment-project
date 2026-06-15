import { authGuard } from './guard/auth.guard';
import { Routes } from '@angular/router';

export const routes: Routes = [
    {
        path: '',
        loadComponent: () => import('./login/login.component').then(m => m.LoginComponent)
    },

    {
        path: 'products',
        canActivate: [authGuard],
        loadComponent: () => import('./products/products.component').then(m => m.ProductsComponent)
    }
];
