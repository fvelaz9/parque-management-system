import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AtraccionForm } from './atraccion-form';

describe('AtraccionForm', () => {
  let component: AtraccionForm;
  let fixture: ComponentFixture<AtraccionForm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AtraccionForm]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AtraccionForm);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
