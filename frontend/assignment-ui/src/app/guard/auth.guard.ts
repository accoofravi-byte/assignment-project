import { inject, Inject } from "@angular/core"; 
import { CanActivate, CanActivateFn, Router } from "@angular/router";

export const authGuard : CanActivateFn = () => {
    const token = localStorage.getItem('token');
    const router = inject(Router);

   if (!token) {
    router.navigate(['/']);
    return false;
  }

  try {
    const payload = JSON.parse(
      atob(token.split('.')[1])
    );

    const currentTime =
      Math.floor(Date.now() / 1000);

    if (payload.exp < currentTime) {

      localStorage.removeItem('token');

      router.navigate(['/']);

      return false;
    }

    return true;

  } catch {

    localStorage.removeItem('token');

    router.navigate(['/']);

    return false;
  }
   };