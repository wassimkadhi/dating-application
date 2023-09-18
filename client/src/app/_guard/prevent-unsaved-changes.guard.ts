import { CanDeactivateFn } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

export const preventUnsavedChangesGuard: CanDeactivateFn<MemberEditComponent> = (
  component) => {
    if(component.editForm.dirty){
      return confirm('Are sure you want to contine ? any unsaved changes will be lost') ;
    }
  return true;
};
