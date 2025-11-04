import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AtraccionEdit } from './atraccion-edit';

describe('AtraccionEdit', () => {
  let component: AtraccionEdit;
  let fixture: ComponentFixture<AtraccionEdit>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AtraccionEdit]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AtraccionEdit);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
